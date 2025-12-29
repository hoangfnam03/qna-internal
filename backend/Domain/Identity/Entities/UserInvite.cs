// Domain/Identity/Entities/UserInvite.cs
using Domain.Common.Entities;

namespace Domain.Identity.Entities
{
    public class UserInvite : EntityBase
    {
        public Guid UserId { get; set; }                // user được mời
        public User? User { get; set; }

        public string TokenHash { get; set; } = default!; // UNIQUE char(64)
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedByUserId { get; set; }       // admin mời
        public User? CreatedByUser { get; set; }

        public string? SentToEmail { get; set; }
    }
}
