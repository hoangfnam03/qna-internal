using Domain.Common.Interfaces;

namespace Domain.Common.Entities
{
    public abstract class AuditableEntity : EntityBase, IAuditableEntity
    {
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedByUserId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
    }
}