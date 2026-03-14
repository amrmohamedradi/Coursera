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
        // The foreign key relation to ApplicationUser will be handled via FluentAPI
        // since ApplicationUser sits in the Infrastructure layer, we can't reference it here.
        
        public bool IsActive => !IsRevoked && ExpiryDate > DateTime.UtcNow;
    }
}
