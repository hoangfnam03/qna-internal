using Domain.Common.Entities;

namespace Domain.Identity.Entities
{
    public class PasswordResetToken : EntityBase
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public string TokenHash { get; set; } = default!; // char(64)
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? SentToEmail { get; set; }
    }
}
