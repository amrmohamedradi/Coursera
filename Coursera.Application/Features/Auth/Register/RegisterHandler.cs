using MediatR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Coursera.Application.Interfaces;
using Coursera.Application.Common.Interfaces;

namespace Coursera.Application.Features.Auth.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        public RegisterHandler(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }
        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.RegisterAsync(request.FirstName, request.LastNeme, request.Email, request.Password);
            var token = await _jwtService.GenerateTokenAsync(user);
            return new AuthResponse
            {
                Token = token,
                Email = user.Email
            };
        }
    }
}
