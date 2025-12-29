// Domain/Identity/Entities/Role.cs
using Domain.Common.Entities;

namespace Domain.Identity.Entities
{
    public class Role : EntityBase
    {
        public string Name { get; set; } = default!;
        public string NormalizedName { get; set; } = default!;
        public string? Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
