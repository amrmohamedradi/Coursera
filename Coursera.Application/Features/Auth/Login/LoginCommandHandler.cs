using Coursera.Application.Common.Interfaces;
using Coursera.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Coursera.Application.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IAuthService _authService;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService,
            IAuthService authService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _authService = authService;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new Exception("Invalid email or password");
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
