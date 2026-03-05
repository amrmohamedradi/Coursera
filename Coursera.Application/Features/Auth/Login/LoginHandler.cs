using Coursera.Application.Common.Interfaces;
using Coursera.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Auth.Login
{
    internal class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        public LoginHandler(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.LoginAsync(request.Email, request.Password);
            var token = await _jwtService.GenerateTokenAsync(user);
            return new AuthResponse
            {
                Token = token,
                Email = user.Email
            };
        }
    }
}
