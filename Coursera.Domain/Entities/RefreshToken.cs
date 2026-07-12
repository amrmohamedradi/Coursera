using System;
using Coursera.Domain.Common;

namespace Coursera.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }

        // Single FK to ApplicationUser — consistent with Order and Cart entities.
        // ApplicationUserId was the column EF registered as the actual FK constraint
        // in the database; UserId was a redundant plain Guid with no FK relationship.
        public Guid UserId { get; set; }

        public bool IsActive => !IsRevoked && ExpiryDate > DateTime.UtcNow;
    }
}
