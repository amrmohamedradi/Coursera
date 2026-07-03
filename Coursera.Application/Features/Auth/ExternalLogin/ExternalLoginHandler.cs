using Coursera.Application.Common.Interfaces;
using Coursera.Application.Common.Models;
using Coursera.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Coursera.Application.Features.Auth.ExternalLogin
{
    public class ExternalLoginHandler : IRequestHandler<ExternalLoginCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<ExternalLoginHandler> _logger;
        private readonly JwtSettings _jwtSettings;

        public ExternalLoginHandler(
            IAuthService authService,
            IJwtService jwtService,
            IOptions<JwtSettings> jwtSettings,
            ILogger<ExternalLoginHandler> logger)
        {
            _authService = authService;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<AuthResponse> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("External login attempt via {Provider}", request.Provider);

            var user = await _authService.ExternalLoginAsync(request.Provider, request.IdToken);

            var token = await _jwtService.GenerateTokenAsync(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);

            await _authService.SetRefreshTokenAsync(user.Id, refreshToken, refreshExpiry);

            _logger.LogInformation("External login successful for {Email} via {Provider}", user.Email, request.Provider);

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                Email = user.Email
            };
        }
    }
}
