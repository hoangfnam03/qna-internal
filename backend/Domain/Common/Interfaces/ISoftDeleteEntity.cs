namespace Domain.Common.Interfaces
{
    public interface ISoftDeleteEntity
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        Guid? DeletedByUserId { get; set; }
    }
}
