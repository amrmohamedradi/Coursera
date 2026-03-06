using Coursera.Application.Common.Interfaces;
using Coursera.Application.Features.Auth.Register;
using Coursera.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
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
        public LoginHandler(IAuthService authService, IJwtService jwtService, ILogger<LoginHandler> logger)
        {
            _authService = authService;
            _jwtService = jwtService;
            _logger = logger;
        }
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for {Email}", request.Email);

            var user = await _authService.LoginAsync(request.Email, request.Password);
            var token = await _jwtService.GenerateTokenAsync(user);
            _logger.LogInformation("USer {Email} Logged successfully", request.Email);

            return new AuthResponse
            {
                Token = token,
                Email = user.Email
            };
        }
    }
}
