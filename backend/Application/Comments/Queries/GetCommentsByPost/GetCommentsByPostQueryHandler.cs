using Application.Common.Interfaces;
using Application.Comments.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Comments.Queries.GetCommentsByPost
{
    public class GetCommentsByPostQueryHandler : IRequestHandler<GetCommentsByPostQuery, List<CommentItemDto>>
    {
        private readonly IApplicationDbContext _db;
        public GetCommentsByPostQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<List<CommentItemDto>> Handle(GetCommentsByPostQuery request, CancellationToken ct)
        {
            var postExists = await _db.Posts.AsNoTracking().AnyAsync(p => p.Id == request.PostId, ct);
            if (!postExists) throw new InvalidOperationException("Post not found.");

            var rows = await (
                from c in _db.Comments.AsNoTracking()
                join u in _db.Users.AsNoTracking() on c.CreatedByUserId equals u.Id
                where c.PostId == request.PostId
                orderby c.CreatedAt
                select new CommentItemDto(
                    c.Id,
                    c.PostId,
                    c.ParentCommentId,
                    c.Body,
                    c.Score,
                    c.CreatedAt,
                    c.UpdatedAt,
                    c.CreatedByUserId,
                    u.DisplayName
                )
            ).ToListAsync(ct);

            return rows;
        }
    }
}
