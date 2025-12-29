using Domain.Communication.Enums;

namespace Application.Notifications.DTOs
{
    public record NotificationItemDto(
        Guid Id,
        NotificationType Type,
        EntityType? EntityType,
        Guid? EntityId,
        string? DataJson,
        bool IsRead,
        DateTime? ReadAt,
        DateTime CreatedAt,
        Guid? ActorUserId,
        string? ActorDisplayName
    );
}
