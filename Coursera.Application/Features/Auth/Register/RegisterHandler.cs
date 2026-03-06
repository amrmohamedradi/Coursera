using Coursera.Application.Common.Interfaces;
using Coursera.Application.Interfaces;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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

        public RegisterHandler(IAuthService authService, IJwtService jwtService, ILogger<RegisterHandler> logger)
        {
            _authService = authService;
            _jwtService = jwtService;
            _logger = logger;
        }
        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Regsiter attempt for {Email}", request.Email);
            var user = await _authService.RegisterAsync(request.FirstName, request.LastNeme, request.Email, request.Password);
            _logger.LogInformation("USer {Email} registered successfully", request.Email);

            var token = await _jwtService.GenerateTokenAsync(user);
            return new AuthResponse
            {
                Token = token,
                Email = user.Email
            };
        }
    }
}
