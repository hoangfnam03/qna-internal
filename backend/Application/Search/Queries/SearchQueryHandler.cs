using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Search.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Search.Queries
{
    public class SearchQueryHandler : IRequestHandler<SearchQuery, SearchResponseDto>
    {
        private readonly IApplicationDbContext _db;
        public SearchQueryHandler(IApplicationDbContext db) => _db = db;

        private static string Excerpt(string? s, int max)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            s = s.Trim();
            return s.Length <= max ? s : s.Substring(0, max);
        }

        public async Task<SearchResponseDto> Handle(SearchQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

            var q = (request.Q ?? "").Trim();
            if (q.Length == 0)
            {
                return new SearchResponseDto(
                    new Paged<PostSearchItemDto>(Array.Empty<PostSearchItemDto>(), page, pageSize, 0),
                    Array.Empty<UserSearchItemDto>(),
                    Array.Empty<TagSearchItemDto>()
                );
            }

            var like = $"%{q}%";

            // ---- POSTS ----
            var postsBase =
                from p in _db.Posts.AsNoTracking()
                join u in _db.Users.AsNoTracking() on p.CreatedByUserId equals u.Id
                where EF.Functions.Like(p.Title, like) || EF.Functions.Like(p.Body, like)
                select new { p, u.DisplayName };

            var totalPosts = await postsBase.CountAsync(ct);

            var rawPosts = await postsBase
                .OrderByDescending(x => x.p.CreatedAt)
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
                    x.DisplayName
                })
                .ToListAsync(ct);

            var posts = rawPosts
                .Select(x => new PostSearchItemDto(
                    x.Id,
                    x.Title,
                    Excerpt(x.Body, 180),
                    x.Score,
                    x.CommentCount,
                    x.CreatedAt,
                    x.CreatedByUserId,
                    x.DisplayName
                ))
                .ToList();


            // ---- USERS (top N) ----
            var users = await _db.Users.AsNoTracking()
                .Where(u =>
                    EF.Functions.Like(u.UserName, like) ||
                    EF.Functions.Like(u.DisplayName, like) ||
                    (u.Email != null && EF.Functions.Like(u.Email, like))
                )
                .OrderBy(u => u.DisplayName)
                .Take(request.UsersTake is < 1 or > 50 ? 10 : request.UsersTake)
                .Select(u => new UserSearchItemDto(u.Id, u.UserName, u.DisplayName, u.Email))
                .ToListAsync(ct);

            // ---- TAGS (top N) ----
            var tags = await _db.Tags.AsNoTracking()
                .Where(t => EF.Functions.Like(t.Name, like) || EF.Functions.Like(t.Slug, like))
                .OrderBy(t => t.Name)
                .Take(request.TagsTake is < 1 or > 50 ? 10 : request.TagsTake)
                .Select(t => new TagSearchItemDto(t.Id, t.Name, t.Slug))
                .ToListAsync(ct);

            return new SearchResponseDto(
                new Paged<PostSearchItemDto>(posts, page, pageSize, totalPosts),
                users,
                tags
            );
        }
    }
}
