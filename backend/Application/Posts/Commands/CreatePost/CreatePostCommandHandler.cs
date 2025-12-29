using Application.Common.Interfaces;
using Domain.Content.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Posts.Commands.CreatePost
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Guid>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public CreatePostCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<Guid> Handle(CreatePostCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var r = request.Request;
            var title = r.Title.Trim();
            var body = r.Body.Trim();

            if (string.IsNullOrWhiteSpace(title)) throw new InvalidOperationException("Title is required.");
            if (string.IsNullOrWhiteSpace(body)) throw new InvalidOperationException("Body is required.");

            if (r.CategoryId.HasValue)
            {
                var ok = await _db.Categories.AsNoTracking().AnyAsync(c => c.Id == r.CategoryId.Value, ct);
                if (!ok) throw new InvalidOperationException("Category not found.");
            }

            var post = new Post
            {
                Title = title,
                Body = body,
                CategoryId = r.CategoryId,
                Score = 0,
                CommentCount = 0,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = _current.UserId.Value
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync(ct);

            var tagIds = (r.TagIds ?? new List<Guid>()).Distinct().ToList();
            if (tagIds.Count > 0)
            {
                var existingTagIds = await _db.Tags.AsNoTracking()
                    .Where(t => tagIds.Contains(t.Id))
                    .Select(t => t.Id)
                    .ToListAsync(ct);

                if (existingTagIds.Count != tagIds.Count)
                    throw new InvalidOperationException("One or more tags not found.");

                foreach (var tagId in tagIds)
                    _db.PostTags.Add(new PostTag { PostId = post.Id, TagId = tagId });

                await _db.SaveChangesAsync(ct);
            }

            return post.Id;
        }
    }
}
