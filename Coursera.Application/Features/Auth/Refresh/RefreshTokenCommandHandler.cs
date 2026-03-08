using Coursera.Application.Common.Interfaces;
using Coursera.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Coursera.Application.Features.Auth.Refresh
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IAuthService _authService;

        public RefreshTokenCommandHandler(
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService,
            IAuthService authService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _authService = authService;
        }

        public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user.RefreshToken != request.RefreshToken)
            {
                throw new Exception("Invalid refresh token");
            }

            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new Exception("Refresh token has expired");
            }

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
