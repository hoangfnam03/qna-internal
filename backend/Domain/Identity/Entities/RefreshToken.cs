// Domain/Identity/Entities/RefreshToken.cs
using Domain.Common.Entities;

namespace Domain.Identity.Entities
{
    public class RefreshToken : EntityBase
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public string TokenHash { get; set; } = default!; // char(64)
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByTokenHash { get; set; }  // char(64)

        public string? Device { get; set; }
        public string? IpAddress { get; set; }
    }
}
