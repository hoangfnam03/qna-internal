using Application.Common.Interfaces;
using Application.Posts.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Posts.Queries.GetPostById
{
    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostDetailDto>
    {
        private readonly IApplicationDbContext _db;
        public GetPostByIdQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<PostDetailDto> Handle(GetPostByIdQuery request, CancellationToken ct)
        {
            var post = await _db.Posts.AsNoTracking()
                .Where(p => p.Id == request.Id)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Body,
                    p.Score,
                    p.CommentCount,
                    p.CreatedAt,
                    p.UpdatedAt,
                    p.CreatedByUserId,
                    p.CategoryId
                })
                .FirstOrDefaultAsync(ct);

            if (post == null) throw new InvalidOperationException("Post not found.");

            var authorName = await _db.Users.AsNoTracking()
                .Where(u => u.Id == post.CreatedByUserId)
                .Select(u => u.DisplayName)
                .FirstOrDefaultAsync(ct) ?? "Unknown";

            string? categoryName = null;
            if (post.CategoryId.HasValue)
            {
                categoryName = await _db.Categories.AsNoTracking()
                    .Where(c => c.Id == post.CategoryId.Value)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync(ct);
            }

            var tags = await (
                from pt in _db.PostTags.AsNoTracking()
                join t in _db.Tags.AsNoTracking() on pt.TagId equals t.Id
                where pt.PostId == post.Id
                orderby t.Name
                select new PostTagDto(t.Id, t.Name, t.Slug)
            ).ToListAsync(ct);

            return new PostDetailDto(
                post.Id,
                post.Title,
                post.Body,
                post.Score,
                post.CommentCount,
                post.CreatedAt,
                post.UpdatedAt,
                post.CreatedByUserId,
                authorName,
                post.CategoryId,
                categoryName,
                tags
            );
        }
    }
}
