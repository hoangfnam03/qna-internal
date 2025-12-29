
using Domain.Common.Entities;
using Domain.Identity.Enums;

namespace Domain.Identity.Entities
{
    public class User : SoftDeletableEntity
    {
        public string UserName { get; set; } = default!;
        public string NormalizedUserName { get; set; } = default!;

        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }

        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }

        public string DisplayName { get; set; } = default!;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }

        public UserStatus Status { get; set; }

        public DateTime? SuspendedUntil { get; set; }
        public string? SuspensionReason { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<UserInvite> UserInvites { get; set; } = new List<UserInvite>();
    }
}
