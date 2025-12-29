using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Comments.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Comments.Queries.GetCommentRevisions
{
    public class GetCommentRevisionsQueryHandler : IRequestHandler<GetCommentRevisionsQuery, Paged<CommentRevisionDto>>
    {
        private readonly IApplicationDbContext _db;
        public GetCommentRevisionsQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<Paged<CommentRevisionDto>> Handle(GetCommentRevisionsQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;

            var q = _db.CommentRevisions.AsNoTracking().Where(r => r.CommentId == request.CommentId);
            var total = await q.CountAsync(ct);

            var items = await (
                from r in q
                join u in _db.Users.AsNoTracking() on r.EditedByUserId equals u.Id
                orderby r.CreatedAt descending
                select new CommentRevisionDto(
                    r.Id,
                    r.CommentId,
                    u.DisplayName,
                    r.CreatedAt,
                    r.Summary,
                    r.BeforeBody,
                    r.AfterBody
                )
            ).Skip((page - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync(ct);

            return new Paged<CommentRevisionDto>(items, page, pageSize, total);
        }
    }
}
