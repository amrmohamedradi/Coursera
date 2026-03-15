using System;
using Coursera.Domain.Common;

namespace Coursera.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public Guid UserId { get; set; }
        public Guid? ApplicationUserId { get; set; }
        public bool IsActive => !IsRevoked && ExpiryDate > DateTime.UtcNow;
    }
}
