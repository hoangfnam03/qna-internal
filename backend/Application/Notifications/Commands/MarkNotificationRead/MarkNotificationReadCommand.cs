using MediatR;

namespace Application.Notifications.Commands.MarkNotificationRead
{
    public record MarkNotificationReadCommand(Guid NotificationId) : IRequest<bool>;
}
