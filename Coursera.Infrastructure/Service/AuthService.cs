using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using Coursera.Domain.Entities;
using Coursera.Infrastructure.Data;
using Coursera.Infrastructure.Identity;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coursera.Infrastructure.Service
{
    /// <summary>
    /// Internal value object returned by each provider validator.
    /// Intentionally kept in Infrastructure — no OAuth concept leaks into Domain or Application.
    /// </summary>
    internal sealed record ExternalUserInfo(
        string Email,
        string FirstName,
        string LastName,
        /// <summary>The provider's stable, unique identifier for this user (Google: "sub", Facebook: "id").</summary>
        string ProviderKey,
        string Provider);

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // ── Standard auth ──────────────────────────────────────────────────────────

        public async Task<UserTokenDto> RegisterAsync(string firstName, string lastName, string email, string password)
        {
            var user = new ApplicationUser(firstName, lastName, email, email);
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new ValidationException(string.Join(",", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "User");
            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            return new UserTokenDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles
            };
        }

        public async Task<UserTokenDto> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                throw new UnauthorizedException("Invalid email or password.");

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            return new UserTokenDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles
            };
        }

        public async Task SetRefreshTokenAsync(Guid userId, string refreshToken, DateTime refreshTokenExpiryTime)
        {
            var user = await _context.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new NotFoundException("User not found.");

            var token = new RefreshToken
            {
                Token = refreshToken,
                ExpiryDate = refreshTokenExpiryTime,
                IsRevoked = false,
                UserId = userId,
                ApplicationUserId = userId
            };

            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<UserTokenDto> RefreshTokenAsync(string email, string refreshToken)
        {
            var user = await _userManager.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new UnauthorizedException("Invalid refresh token.");

            var activeToken = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
            if (activeToken == null || !activeToken.IsActive)
                throw new UnauthorizedException("Invalid or expired refresh token.");

            activeToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            return new UserTokenDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles
            };
        }

        // ── External / Social login ────────────────────────────────────────────────

        /// <summary>
        /// Validates an ID token or access token issued by an external provider,
        /// then finds or provisions the local user account via the three-step lookup:
        ///   1. FindByLoginAsync(provider, providerKey)  → already linked, return immediately
        ///   2. FindByEmailAsync(email)                  → existing local account, link it
        ///   3. CreateAsync + AddLoginAsync              → brand-new user, provision and link
        /// </summary>
        public async Task<UserTokenDto> ExternalLoginAsync(string provider, string idToken)
        {
            // --- Step A: validate with the provider, get stable user info ---
            var info = provider.ToLowerInvariant() switch
            {
                "google" => await ValidateGoogleTokenAsync(idToken),
                "facebook" => await ValidateFacebookTokenAsync(idToken),
                _ => throw new UnauthorizedException($"Unsupported provider: {provider}.")
            };

            _logger.LogInformation(
                "External login validated. Provider={Provider}, ProviderKey={ProviderKey}, Email={Email}",
                info.Provider, info.ProviderKey, info.Email);

            var loginInfo = new UserLoginInfo(info.Provider, info.ProviderKey, info.Provider);

            // --- Step B: 3-step find-or-provision ---

            // 1. Happy path — user already linked this social provider
            var user = await _userManager.FindByLoginAsync(info.Provider, info.ProviderKey);

            if (user == null)
            {
                // 2. Existing account registered via email/password (or a different provider)
                //    → link the new provider to the existing account
                user = await _userManager.FindByEmailAsync(info.Email);

                if (user != null)
                {
                    _logger.LogInformation(
                        "Linking {Provider} login to existing account {Email}.", info.Provider, info.Email);

                    var linkResult = await _userManager.AddLoginAsync(user, loginInfo);
                    if (!linkResult.Succeeded)
                    {
                        // The login is already linked to this user — benign race condition; continue.
                        var alreadyLinked = linkResult.Errors.All(e => e.Code == "LoginAlreadyAssociated");
                        if (!alreadyLinked)
                            throw new ValidationException(string.Join(", ", linkResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    // 3. First time ever — create a new account with no password (social-only)
                    _logger.LogInformation(
                        "Creating new user for external login. Provider={Provider}, Email={Email}",
                        info.Provider, info.Email);

                    user = new ApplicationUser(info.FirstName, info.LastName, info.Email, info.Email);
                    var createResult = await _userManager.CreateAsync(user);

                    if (!createResult.Succeeded)
                    {
                        // Guard against duplicate email race condition (concurrent first-time logins)
                        var isDuplicate = createResult.Errors.Any(e =>
                            e.Code is "DuplicateUserName" or "DuplicateEmail");

                        if (isDuplicate)
                        {
                            // Another request just created this user — retry lookup
                            user = await _userManager.FindByEmailAsync(info.Email)
                                   ?? throw new UnauthorizedException("Unable to provision user account. Please try again.");
                        }
                        else
                        {
                            throw new ValidationException(string.Join(", ", createResult.Errors.Select(e => e.Description)));
                        }
                    }

                    await _userManager.AddToRoleAsync(user, "User");

                    var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                    if (!addLoginResult.Succeeded)
                    {
                        // Race condition: another request already added the login — tolerate it
                        var alreadyLinked = addLoginResult.Errors.All(e => e.Code == "LoginAlreadyAssociated");
                        if (!alreadyLinked)
                            throw new ValidationException(string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            return new UserTokenDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles
            };
        }

        // ── Private provider validators ────────────────────────────────────────────

        /// <summary>
        /// Validates a Google ID token (credential) returned by the Google Identity SDK.
        /// Uses Google.Apis.Auth for cryptographic offline validation — no HTTP round-trip
        /// to Google's /tokeninfo endpoint. Google certs are cached and rotated automatically.
        /// </summary>
        private async Task<ExternalUserInfo> ValidateGoogleTokenAsync(string idToken)
        {
            var clientId = _configuration["ExternalAuth:Google:ClientId"];

            if (string.IsNullOrWhiteSpace(clientId) || clientId == "REPLACE_WITH_GOOGLE_CLIENT_ID")
                throw new UnauthorizedException("Google authentication is not configured on this server.");

            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    // Enforce audience = our ClientId — reject tokens issued for other apps
                    Audience = new[] { clientId }
                };

                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning("Google token validation failed: {Message}", ex.Message);
                throw new UnauthorizedException("Invalid or expired Google token.");
            }

            if (string.IsNullOrWhiteSpace(payload.Email))
                throw new UnauthorizedException("Google account does not expose an email address.");

            // payload.Subject is Google's stable, unique identifier ("sub" claim)
            return new ExternalUserInfo(
                Email: payload.Email,
                FirstName: payload.GivenName ?? "User",
                LastName: payload.FamilyName ?? "User",
                ProviderKey: payload.Subject,
                Provider: "google");
        }

        /// <summary>
        /// Validates a Facebook user access token via the Graph API debug_token endpoint.
        /// Also validates that the token was issued for THIS app (app_id check) to prevent
        /// cross-application token injection attacks.
        /// </summary>
        private async Task<ExternalUserInfo> ValidateFacebookTokenAsync(string accessToken)
        {
            var appId = _configuration["ExternalAuth:Facebook:AppId"];
            var appSecret = _configuration["ExternalAuth:Facebook:AppSecret"];

            if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(appSecret)
                || appId == "REPLACE_WITH_FACEBOOK_APP_ID" || appSecret == "REPLACE_WITH_FACEBOOK_APP_SECRET")
                throw new UnauthorizedException("Facebook authentication is not configured on this server.");

            var client = _httpClientFactory.CreateClient();

            // 1) Validate the token against our app token and check it was issued for OUR app_id
            //    Using app_id|app_secret as the app token avoids issuing a separate app access token call.
            var appToken = $"{appId}|{appSecret}";
            var debugUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appToken}";
            var debugResponse = await client.GetAsync(debugUrl);

            if (!debugResponse.IsSuccessStatusCode)
                throw new UnauthorizedException("Facebook token validation request failed.");

            var debugJson = await debugResponse.Content.ReadAsStringAsync();
            using var debugDoc = JsonDocument.Parse(debugJson);
            var data = debugDoc.RootElement.GetProperty("data");

            // is_valid: token is active and not expired
            var isValid = data.TryGetProperty("is_valid", out var isValidProp) && isValidProp.GetBoolean();
            if (!isValid)
                throw new UnauthorizedException("Facebook token is invalid or has been revoked.");

            // app_id: the token must be issued for THIS application — prevents token injection from other FB apps
            var returnedAppId = data.TryGetProperty("app_id", out var appIdProp) ? appIdProp.GetString() : null;
            if (returnedAppId != appId)
            {
                _logger.LogWarning(
                    "Facebook token app_id mismatch. Expected={Expected}, Got={Got}", appId, returnedAppId);
                throw new UnauthorizedException("Facebook token was not issued for this application.");
            }

            // user_id: Facebook's stable unique identifier for this user
            var providerKey = data.TryGetProperty("user_id", out var userIdProp) ? userIdProp.GetString() : null;
            if (string.IsNullOrWhiteSpace(providerKey))
                throw new UnauthorizedException("Could not determine Facebook user identity.");

            // 2) Fetch profile — email, name, and stable user id
            var profileUrl = $"https://graph.facebook.com/me?fields=email,first_name,last_name,id&access_token={accessToken}";
            var profileResponse = await client.GetAsync(profileUrl);

            if (!profileResponse.IsSuccessStatusCode)
                throw new UnauthorizedException("Could not retrieve Facebook user profile.");

            var profileJson = await profileResponse.Content.ReadAsStringAsync();
            using var profileDoc = JsonDocument.Parse(profileJson);
            var profile = profileDoc.RootElement;

            var email = profile.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
            if (string.IsNullOrWhiteSpace(email))
                throw new UnauthorizedException(
                    "Facebook account does not have a public email address. " +
                    "Please register with your email and password instead.");

            var firstName = profile.TryGetProperty("first_name", out var fnProp) ? fnProp.GetString() ?? "User" : "User";
            var lastName = profile.TryGetProperty("last_name", out var lnProp) ? lnProp.GetString() ?? "User" : "User";

            return new ExternalUserInfo(
                Email: email,
                FirstName: firstName,
                LastName: lastName,
                ProviderKey: providerKey,
                Provider: "facebook");
        }
    }
}
