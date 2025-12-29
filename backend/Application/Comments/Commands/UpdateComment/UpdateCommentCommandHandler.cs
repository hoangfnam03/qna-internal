using Application.Common.Interfaces;
using Domain.Content.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Comments.Commands.UpdateComment
{
    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public UpdateCommentCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<bool> Handle(UpdateCommentCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var uid = _current.UserId.Value;
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId, ct);
            if (comment == null) throw new InvalidOperationException("Comment not found.");

            var canEdit = request.IsAdmin || comment.CreatedByUserId == uid;
            if (!canEdit) throw new UnauthorizedAccessException("You cannot edit this comment.");

            var newBody = request.Request.Body?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(newBody)) throw new InvalidOperationException("Body is required.");

            var before = comment.Body;

            comment.Body = newBody;
            comment.UpdatedAt = DateTime.UtcNow;
            comment.UpdatedByUserId = uid;

            _db.CommentRevisions.Add(new CommentRevision
            {
                CommentId = comment.Id,
                BeforeBody = before,
                AfterBody = newBody,
                Summary = string.IsNullOrWhiteSpace(request.Request.Summary) ? null : request.Request.Summary.Trim(),
                EditedByUserId = uid,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
