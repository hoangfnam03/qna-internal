using Application.Common.Interfaces;
using Application.Moderation.DTOs;
using Domain.Communication.Entities;
using Domain.Communication.Enums;
using Domain.Content.Entities;
using Domain.Moderation.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Application.Moderation.Commands.AdminDeleteTarget
{
    public class AdminDeleteTargetCommandHandler : IRequestHandler<AdminDeleteTargetCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public AdminDeleteTargetCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db; _current = current;
        }

        public async Task<bool> Handle(AdminDeleteTargetCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            var adminId = _current.UserId.Value;

            var r = request.Request;
            var note = string.IsNullOrWhiteSpace(r.Note) ? null : r.Note.Trim();

            Guid? notifyUserId = null;   // chủ entity để notify
            EntityType? entityType = null;
            Guid? entityId = null;

            switch (r.TargetType)
            {
                case ReportTargetType.Post:
                    {
                        var post = await _db.Posts.FirstOrDefaultAsync(x => x.Id == r.TargetId, ct);
                        if (post == null) throw new InvalidOperationException("Post not found.");

                        // Soft delete
                        _db.Posts.Remove(post); // interceptor sẽ chuyển thành IsDeleted = true
                        notifyUserId = post.CreatedByUserId;
                        entityType = EntityType.Post;
                        entityId = post.Id;
                        break;
                    }

                case ReportTargetType.Comment:
                    {
                        var cmt = await _db.Comments.FirstOrDefaultAsync(x => x.Id == r.TargetId, ct);
                        if (cmt == null) throw new InvalidOperationException("Comment not found.");

                        _db.Comments.Remove(cmt);
                        notifyUserId = cmt.CreatedByUserId;
                        entityType = EntityType.Comment;
                        entityId = cmt.Id;
                        break;
                    }

                case ReportTargetType.User:
                    {
                        // User soft delete
                        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == r.TargetId, ct);
                        if (user == null) throw new InvalidOperationException("User not found.");

                        _db.Users.Remove(user);
                        notifyUserId = user.Id;
                        entityType = EntityType.User;
                        entityId = user.Id;
                        break;
                    }

                case ReportTargetType.Announcement:
                    {
                        var a = await _db.Announcements.FirstOrDefaultAsync(x => x.Id == r.TargetId, ct);
                        if (a == null) throw new InvalidOperationException("Announcement not found.");

                        _db.Announcements.Remove(a);
                        notifyUserId = a.CreatedByUserId;
                        entityType = EntityType.Announcement;
                        entityId = a.Id;
                        break;
                    }

                default:
                    throw new InvalidOperationException("Unsupported target type.");
            }

            // notify owner (nếu có và không phải admin)
            if (notifyUserId.HasValue && notifyUserId.Value != adminId)
            {
                var payload = JsonSerializer.Serialize(new
                {
                    action = "Deleted",
                    targetType = r.TargetType.ToString(),
                    targetId = r.TargetId,
                    note
                });

                _db.Notifications.Add(new Notification
                {
                    RecipientUserId = notifyUserId.Value,
                    ActorUserId = adminId,
                    Type = NotificationType.ReportStatusChanged,
                    EntityType = entityType,                    
                    EntityId = entityId,
                    DataJson = payload,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
