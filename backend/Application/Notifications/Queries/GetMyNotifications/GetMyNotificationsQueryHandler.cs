using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Notifications.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.Queries.GetMyNotifications
{
    public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Paged<NotificationItemDto>>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public GetMyNotificationsQueryHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<Paged<NotificationItemDto>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            var uid = _current.UserId.Value;

            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 200 ? 20 : request.PageSize;

            var q = _db.Notifications.AsNoTracking().Where(n => n.RecipientUserId == uid);

            if (request.UnreadOnly)
                q = q.Where(n => !n.IsRead);

            var total = await q.CountAsync(ct);

            var items = await (
                from n in q
                join u in _db.Users.AsNoTracking() on n.ActorUserId equals u.Id into actorJoin
                from actor in actorJoin.DefaultIfEmpty()
                orderby n.CreatedAt descending
                select new NotificationItemDto(
                    n.Id,
                    n.Type,
                    n.EntityType,
                    n.EntityId,
                    n.DataJson,
                    n.IsRead,
                    n.ReadAt,
                    n.CreatedAt,
                    n.ActorUserId,
                    actor != null ? actor.DisplayName : null
                )
            ).Skip((page - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync(ct);

            return new Paged<NotificationItemDto>(items, page, pageSize, total);
        }
    }
}
