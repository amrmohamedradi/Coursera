using Coursera.Application.Common.Interfaces;
using Coursera.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Coursera.Application.Common.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Auth.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<RegisterHandler> _logger;
        private readonly JwtSettings _jwtSettings;

        public RegisterHandler(IAuthService authService, IJwtService jwtService, IOptions<JwtSettings> jwtSettings, ILogger<RegisterHandler> logger)
        {
            _authService = authService;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }
        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Register attempt for {Email}", request.Email);
            var user = await _authService.RegisterAsync(request.FirstName, request.LastName, request.Email, request.Password);
            _logger.LogInformation("User {Email} registered successfully", request.Email);

            var token = await _jwtService.GenerateTokenAsync(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);
            await _authService.SetRefreshTokenAsync(user.Id, refreshToken, refreshExpiry);
            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                Email = user.Email
            };
        }
    }
}
