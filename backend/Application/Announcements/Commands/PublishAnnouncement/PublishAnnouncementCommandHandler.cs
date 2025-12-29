using System.Text.Json;
using Application.Common.Interfaces;
using Domain.Communication.Entities;
using Domain.Communication.Enums;
using Domain.Identity.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Announcements.Commands.PublishAnnouncement
{
    public class PublishAnnouncementCommandHandler : IRequestHandler<PublishAnnouncementCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public PublishAnnouncementCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<bool> Handle(PublishAnnouncementCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            var actorId = _current.UserId.Value;

            var a = await _db.Announcements.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (a == null) throw new InvalidOperationException("Announcement not found.");

            if (!a.IsPublished)
            {
                a.IsPublished = true;
                a.PublishedAt = DateTime.UtcNow;
            }

            a.UpdatedAt = DateTime.UtcNow;
            a.UpdatedByUserId = actorId;

            // notify all active users (exclude actor)
            // NOTE: User.Status trong domain của bạn đang là byte => so sánh cast
            var activeUsers = await _db.Users.AsNoTracking()
                .Where(u => u.Id != actorId && u.Status == UserStatus.Active)
                .Select(u => u.Id)
                .ToListAsync(ct);

            var payload = JsonSerializer.Serialize(new
            {
                announcementId = a.Id,
                title = a.Title,
                publishedAt = a.PublishedAt
            });

            const int batchSize = 500;
            for (int i = 0; i < activeUsers.Count; i += batchSize)
            {
                var batch = activeUsers.Skip(i).Take(batchSize);
                var notis = batch.Select(uid => new Notification
                {
                    RecipientUserId = uid,
                    ActorUserId = actorId,
                    Type = NotificationType.AnnouncementPublished,
                    EntityType = EntityType.Announcement,
                    EntityId = a.Id,
                    DataJson = payload,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                _db.Notifications.AddRange(notis);
                await _db.SaveChangesAsync(ct);
            }

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
