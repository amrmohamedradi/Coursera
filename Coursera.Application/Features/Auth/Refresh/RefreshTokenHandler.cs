using Coursera.Application.Common.Interfaces;
using Coursera.Application.Common.Models;
using Coursera.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Coursera.Application.Features.Auth.Refresh
{
    internal class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<RefreshTokenHandler> _logger;

        public RefreshTokenHandler(IAuthService authService, IJwtService jwtService, IOptions<JwtSettings> jwtSettings, ILogger<RefreshTokenHandler> logger)
        {
            _authService = authService;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Refresh token attempt for {Email}", request.Email);

            var user = await _authService.RefreshTokenAsync(request.Email, request.RefreshToken);

            var token = await _jwtService.GenerateTokenAsync(user);

            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var refreshExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);
            await _authService.SetRefreshTokenAsync(user.Id, newRefreshToken, refreshExpiry);

            return new AuthResponse
            {
                Token = token,
                RefreshToken = newRefreshToken,
                Email = user.Email
            };
        }
    }
}
