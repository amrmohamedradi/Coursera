using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Auth.Login
{
    public class LoginCommand : IRequest<AuthResponse>
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
