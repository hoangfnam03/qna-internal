using System.Text.Json;
using Application.Common.Interfaces;
using Application.Comments.DTOs;
using Domain.Communication.Entities;
using Domain.Communication.Enums; // NotificationType, EntityType
using Domain.Content.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Comments.Commands.CreateComment
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Guid>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public CreateCommentCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var uid = _current.UserId.Value;
            var body = request.Request.Body?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(body)) throw new InvalidOperationException("Body is required.");

            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId, ct);
            if (post == null) throw new InvalidOperationException("Post not found.");

            if (request.Request.ParentCommentId.HasValue)
            {
                var parentId = request.Request.ParentCommentId.Value;
                var parentOk = await _db.Comments.AsNoTracking()
                    .AnyAsync(c => c.Id == parentId && c.PostId == request.PostId, ct);
                if (!parentOk) throw new InvalidOperationException("Parent comment not found (or not in this post).");
            }

            var comment = new Comment
            {
                PostId = request.PostId,
                ParentCommentId = request.Request.ParentCommentId,
                Body = body,
                Score = 0,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = uid
            };

            _db.Comments.Add(comment);

            // cache
            post.CommentCount += 1;

            // notification to post author (if not self)
            if (post.CreatedByUserId != uid)
            {
                var payload = JsonSerializer.Serialize(new
                {
                    postId = post.Id,
                    commentId = comment.Id,
                    postTitle = post.Title
                });

                _db.Notifications.Add(new Notification
                {
                    RecipientUserId = post.CreatedByUserId,
                    ActorUserId = uid,
                    Type = NotificationType.CommentCreated,
                    EntityType = EntityType.Comment,
                    EntityId = comment.Id,
                    DataJson = payload,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _db.SaveChangesAsync(ct);
            return comment.Id;
        }
    }
}
