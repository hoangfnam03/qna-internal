using Application.Common.Models;
using Application.Notifications.DTOs;
using MediatR;

namespace Application.Notifications.Queries.GetMyNotifications
{
    public record GetMyNotificationsQuery(bool UnreadOnly, int Page = 1, int PageSize = 20)
        : IRequest<Paged<NotificationItemDto>>;
}
