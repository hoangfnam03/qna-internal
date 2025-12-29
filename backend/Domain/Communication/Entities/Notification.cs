// Domain/Communication/Entities/Notification.cs
using Domain.Common.Entities;
using Domain.Communication.Enums;
using Domain.Identity.Entities;

namespace Domain.Communication.Entities
{
    public class Notification : EntityBase
    {
        public Guid RecipientUserId { get; set; }
        public User? RecipientUser { get; set; }

        public Guid? ActorUserId { get; set; }
        public User? ActorUser { get; set; }

        public NotificationType Type { get; set; }
        public EntityType? EntityType { get; set; }
        public Guid? EntityId { get; set; }

        public string? DataJson { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
