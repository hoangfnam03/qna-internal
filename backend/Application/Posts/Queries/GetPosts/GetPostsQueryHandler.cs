using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Posts.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Posts.Queries.GetPosts
{
    public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, Paged<PostListItemDto>>
    {
        private readonly IApplicationDbContext _db;
        public GetPostsQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<Paged<PostListItemDto>> Handle(GetPostsQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 200 ? 20 : request.PageSize;

            var q = _db.Posts.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.Trim();
                q = q.Where(p => p.Title.Contains(s) || p.Body.Contains(s));
            }

            if (request.CategoryId.HasValue)
                q = q.Where(p => p.CategoryId == request.CategoryId.Value);

            if (request.AuthorId.HasValue)
                q = q.Where(p => p.CreatedByUserId == request.AuthorId.Value);

            if (request.TagId.HasValue)
            {
                var tagId = request.TagId.Value;
                q = q.Where(p => _db.PostTags.Any(pt => pt.PostId == p.Id && pt.TagId == tagId));
            }

            // Sort
            var sort = (request.Sort ?? "recent").Trim().ToLowerInvariant();
            q = sort switch
            {
                "popular" => q.OrderByDescending(p => p.Score).ThenByDescending(p => p.CreatedAt),
                "unanswered" => q.Where(p => p.CommentCount == 0).OrderByDescending(p => p.CreatedAt),
                _ => q.OrderByDescending(p => p.CreatedAt)
            };

            var total = await q.CountAsync(ct);

            // Page items (project minimal first)
            var pageItems = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Body,
                    p.Score,
                    p.CommentCount,
                    p.CreatedAt,
                    p.CreatedByUserId,
                    p.CategoryId
                })
                .ToListAsync(ct);

            var postIds = pageItems.Select(x => x.Id).ToList();
            var authorIds = pageItems.Select(x => x.CreatedByUserId).Distinct().ToList();
            var categoryIds = pageItems.Where(x => x.CategoryId != null).Select(x => x.CategoryId!.Value).Distinct().ToList();

            var authors = await _db.Users.AsNoTracking()
                .Where(u => authorIds.Contains(u.Id))
                .Select(u => new { u.Id, u.DisplayName })
                .ToDictionaryAsync(x => x.Id, x => x.DisplayName, ct);

            var categories = await _db.Categories.AsNoTracking()
                .Where(c => categoryIds.Contains(c.Id))
                .Select(c => new { c.Id, c.Name })
                .ToDictionaryAsync(x => x.Id, x => x.Name, ct);

            var tagRows = await (
                from pt in _db.PostTags.AsNoTracking()
                join t in _db.Tags.AsNoTracking() on pt.TagId equals t.Id
                where postIds.Contains(pt.PostId)
                select new { pt.PostId, Tag = new PostTagDto(t.Id, t.Name, t.Slug) }
            ).ToListAsync(ct);

            var tagsByPost = tagRows
                .GroupBy(x => x.PostId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Tag).OrderBy(x => x.Name).ToList());

            static string Excerpt(string s, int max = 180)
            {
                if (string.IsNullOrWhiteSpace(s)) return "";
                s = s.Trim();
                return s.Length <= max ? s : s.Substring(0, max);
            }

            var items = pageItems.Select(x =>
            {
                authors.TryGetValue(x.CreatedByUserId, out var authorName);
                string? categoryName = null;
                if (x.CategoryId.HasValue) categories.TryGetValue(x.CategoryId.Value, out categoryName);

                tagsByPost.TryGetValue(x.Id, out var tags);
                tags ??= new List<PostTagDto>();

                return new PostListItemDto(
                    x.Id,
                    x.Title,
                    Excerpt(x.Body),
                    x.Score,
                    x.CommentCount,
                    x.CreatedAt,
                    x.CreatedByUserId,
                    authorName ?? "Unknown",
                    x.CategoryId,
                    categoryName,
                    tags
                );
            }).ToList();

            return new Paged<PostListItemDto>(items, page, pageSize, total);
        }
    }
}
