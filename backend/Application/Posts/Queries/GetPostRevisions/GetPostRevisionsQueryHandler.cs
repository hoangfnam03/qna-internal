using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Posts.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Posts.Queries.GetPostRevisions
{
    public class GetPostRevisionsQueryHandler : IRequestHandler<GetPostRevisionsQuery, Paged<PostRevisionDto>>
    {
        private readonly IApplicationDbContext _db;
        public GetPostRevisionsQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<Paged<PostRevisionDto>> Handle(GetPostRevisionsQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;

            var q = _db.PostRevisions.AsNoTracking().Where(r => r.PostId == request.PostId);

            var total = await q.CountAsync(ct);

            var rows = await (
                from r in q
                join u in _db.Users.AsNoTracking() on r.EditedByUserId equals u.Id
                orderby r.CreatedAt descending
                select new PostRevisionDto(
                    r.Id,
                    r.PostId,
                    u.DisplayName,
                    r.CreatedAt,
                    r.Summary,
                    r.BeforeTitle,
                    r.AfterTitle,
                    r.BeforeBody,
                    r.AfterBody
                )
            ).Skip((page - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync(ct);

            return new Paged<PostRevisionDto>(rows, page, pageSize, total);
        }
    }
}
