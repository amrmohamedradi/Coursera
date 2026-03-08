using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
