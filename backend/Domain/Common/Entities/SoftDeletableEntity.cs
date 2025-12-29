using Domain.Common.Interfaces;

namespace Domain.Common.Entities
{
    public abstract class SoftDeletableEntity : AuditableEntity, ISoftDeleteEntity
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedByUserId { get; set; }
    }
}
