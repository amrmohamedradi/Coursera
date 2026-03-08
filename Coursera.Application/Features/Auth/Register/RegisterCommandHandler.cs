using Coursera.Application.Common.Interfaces;
using Coursera.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Coursera.Application.Features.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IAuthService _authService;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService,
            IAuthService authService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _authService = authService;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new Exception("User with this email already exists");
            }

            var user = new ApplicationUser(request.FirstName, request.LastName, request.Email, request.Email);
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await _userManager.AddToRoleAsync(user, "User");

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _authService.SetRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiryTime);

            return new AuthResponse
            {
                Email = user.Email!,
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
