using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using Coursera.Domain.Entities;
using Coursera.Infrastructure.Data;
using Coursera.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coursera.Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }


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

        public async Task<UserTokenDto> ExternalLoginAsync(string provider, string idToken)
        {
            var (email, firstName, lastName) = provider.ToLowerInvariant() switch
            {
                "google" => await ValidateGoogleTokenAsync(idToken),
                "facebook" => await ValidateFacebookTokenAsync(idToken),
                _ => throw new UnauthorizedException($"Unsupported provider: {provider}.")
            };

            // Find existing user or create a new one
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Generate a random password — this user can only sign in via external provider
                var randomPassword = $"Ext@{Guid.NewGuid():N}";
                user = new ApplicationUser(firstName, lastName, email, email);
                var result = await _userManager.CreateAsync(user, randomPassword);
                if (!result.Succeeded)
                    throw new ValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(user, "User");
            }

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            return new UserTokenDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles
            };
        }

        // ── Private helpers ────────────────────────────────────────────────────────

        private async Task<(string Email, string FirstName, string LastName)> ValidateGoogleTokenAsync(string idToken)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");

            if (!response.IsSuccessStatusCode)
                throw new UnauthorizedException("Invalid Google token.");

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Validate audience matches our configured ClientId
            var configuredClientId = _configuration["ExternalAuth:Google:ClientId"];
            if (!string.IsNullOrEmpty(configuredClientId) && configuredClientId != "REPLACE_WITH_GOOGLE_CLIENT_ID")
            {
                var audience = root.TryGetProperty("aud", out var audProp) ? audProp.GetString() : null;
                if (audience != configuredClientId)
                    throw new UnauthorizedException("Google token audience mismatch.");
            }

            var email = root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedException("Google token does not contain an email address.");

            var firstName = root.TryGetProperty("given_name", out var fnProp) ? fnProp.GetString() ?? "User" : "User";
            var lastName = root.TryGetProperty("family_name", out var lnProp) ? lnProp.GetString() ?? "User" : "User";

            return (email, firstName, lastName);
        }

        private async Task<(string Email, string FirstName, string LastName)> ValidateFacebookTokenAsync(string accessToken)
        {
            var appId = _configuration["ExternalAuth:Facebook:AppId"];
            var appSecret = _configuration["ExternalAuth:Facebook:AppSecret"];

            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appSecret)
                || appId == "REPLACE_WITH_FACEBOOK_APP_ID" || appSecret == "REPLACE_WITH_FACEBOOK_APP_SECRET")
                throw new UnauthorizedException("Facebook authentication is not configured on this server.");

            var client = _httpClientFactory.CreateClient();

            // 1) Validate the user token against our app token
            var appToken = $"{appId}|{appSecret}";
            var debugUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appToken}";
            var debugResponse = await client.GetAsync(debugUrl);

            if (!debugResponse.IsSuccessStatusCode)
                throw new UnauthorizedException("Invalid Facebook token.");

            var debugJson = await debugResponse.Content.ReadAsStringAsync();
            using var debugDoc = JsonDocument.Parse(debugJson);
            var data = debugDoc.RootElement.GetProperty("data");

            var isValid = data.TryGetProperty("is_valid", out var isValidProp) && isValidProp.GetBoolean();
            if (!isValid)
                throw new UnauthorizedException("Facebook token validation failed.");

            // 2) Fetch user profile
            var profileUrl = $"https://graph.facebook.com/me?fields=email,first_name,last_name&access_token={accessToken}";
            var profileResponse = await client.GetAsync(profileUrl);

            if (!profileResponse.IsSuccessStatusCode)
                throw new UnauthorizedException("Could not retrieve Facebook user profile.");

            var profileJson = await profileResponse.Content.ReadAsStringAsync();
            using var profileDoc = JsonDocument.Parse(profileJson);
            var profile = profileDoc.RootElement;

            var email = profile.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedException("Facebook account does not have a public email address.");

            var firstName = profile.TryGetProperty("first_name", out var fnProp) ? fnProp.GetString() ?? "User" : "User";
            var lastName = profile.TryGetProperty("last_name", out var lnProp) ? lnProp.GetString() ?? "User" : "User";

            return (email, firstName, lastName);
        }
    }
}
