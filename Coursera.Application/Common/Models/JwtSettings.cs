using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.Models
{
    public class JwtSettings
    {
        public string Key { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int DurationInHours { get; set; }
        public int RefreshTokenDurationInDays { get; set; }
    }
}
