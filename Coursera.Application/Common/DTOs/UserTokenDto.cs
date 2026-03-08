using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.DTOs
{
    public class UserTokenDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public List<string> Roles { get; set; } = new List<string>();
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

    }
}
