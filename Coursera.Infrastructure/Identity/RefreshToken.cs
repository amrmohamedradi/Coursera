using System;

namespace Coursera.Infrastructure.Identity
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public bool IsActive => !IsRevoked && ExpiryDate > DateTime.UtcNow;
    }
}
