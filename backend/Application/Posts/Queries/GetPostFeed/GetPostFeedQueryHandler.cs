using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Posts.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Posts.Queries.GetPostFeed
{
    public class GetPostFeedQueryHandler : IRequestHandler<GetPostFeedQuery, Paged<PostFeedItemDto>>
    {
        private readonly IApplicationDbContext _db;
        public GetPostFeedQueryHandler(IApplicationDbContext db) => _db = db;

        private static string Excerpt(string? s, int max)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim();
            return s.Length <= max ? s : s.Substring(0, max);
        }

        public async Task<Paged<PostFeedItemDto>> Handle(GetPostFeedQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var q =
                from p in _db.Posts.AsNoTracking()
                join u in _db.Users.AsNoTracking() on p.CreatedByUserId equals u.Id
                select new { p, AuthorName = u.DisplayName };

            if (request.AuthorId.HasValue)
                q = q.Where(x => x.p.CreatedByUserId == request.AuthorId.Value);

            if (string.Equals(request.Filter, "unanswered", StringComparison.OrdinalIgnoreCase))
                q = q.Where(x => x.p.CommentCount == 0);

            // sort
            if (string.Equals(request.Sort, "popular", StringComparison.OrdinalIgnoreCase))
                q = q.OrderByDescending(x => x.p.Score).ThenByDescending(x => x.p.CreatedAt);
            else
                q = q.OrderByDescending(x => x.p.CreatedAt); // default recent

            var total = await q.CountAsync(ct);

            var raw = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.p.Id,
                x.p.Title,
                x.p.Body,
                x.p.Score,
                x.p.CommentCount,
                x.p.CreatedAt,
                x.p.CreatedByUserId,
                x.AuthorName
            })
            .ToListAsync(ct);

            var items = raw.Select(x => new PostFeedItemDto(
                x.Id,
                x.Title,
                Excerpt(x.Body, 180),
                x.Score,
                x.CommentCount,
                x.CreatedAt,
                x.CreatedByUserId,
                x.AuthorName
            )).ToList();

            return new Paged<PostFeedItemDto>(items, page, pageSize, total);
        }
    }
}
