using Application.Common.Interfaces;
using Domain.Content.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Comments.Commands.VoteComment
{
    public class VoteCommentCommandHandler : IRequestHandler<VoteCommentCommand, int>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public VoteCommentCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<int> Handle(VoteCommentCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            var uid = _current.UserId.Value;

            var value = request.Request.Value;
            if (value != -1 && value != 1) throw new InvalidOperationException("Vote value must be -1 or 1.");

            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId, ct);
            if (comment == null) throw new InvalidOperationException("Comment not found.");

            var vote = await _db.CommentVotes.FirstOrDefaultAsync(v => v.CommentId == comment.Id && v.UserId == uid, ct);

            if (vote == null)
            {
                _db.CommentVotes.Add(new CommentVote
                {
                    CommentId = comment.Id,
                    UserId = uid,
                    Value = value,
                    CreatedAt = DateTime.UtcNow
                });
                comment.Score += value;
            }
            else
            {
                if (vote.Value == value) return comment.Score; // no change

                // adjust score by delta
                var delta = value - vote.Value;
                vote.Value = value;
                comment.Score += delta;
            }

            await _db.SaveChangesAsync(ct);
            return comment.Score;
        }
    }
}
