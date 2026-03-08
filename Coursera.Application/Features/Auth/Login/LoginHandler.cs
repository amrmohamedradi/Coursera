using Coursera.Application.Common.Interfaces;
using Coursera.Application.Features.Auth.Register;
using Coursera.Application.Common.Models;
using Coursera.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Auth.Login
{
    internal class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginHandler> _logger;
        private readonly JwtSettings _jwtSettings;
        public LoginHandler(IAuthService authService, IJwtService jwtService, IOptions<JwtSettings> jwtSettings, ILogger<LoginHandler> logger)
        {
            _authService = authService;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for {Email}", request.Email);

            var user = await _authService.LoginAsync(request.Email, request.Password);
            var token = await _jwtService.GenerateTokenAsync(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);
            await _authService.SetRefreshTokenAsync(user.Id, refreshToken, refreshExpiry);
            _logger.LogInformation("USer {Email} Logged successfully", request.Email);

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                Email = user.Email
            };
        }
    }
}
