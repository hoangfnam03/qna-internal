using Application.Common.Interfaces;
using Domain.Content.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Posts.Commands.UpdatePost
{
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public UpdatePostCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        public async Task<bool> Handle(UpdatePostCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
            if (post == null) throw new InvalidOperationException("Post not found.");

            var uid = _current.UserId.Value;
            var canEdit = request.IsAdmin || post.CreatedByUserId == uid;
            if (!canEdit) throw new UnauthorizedAccessException("You cannot edit this post.");

            var r = request.Request;

            var newTitle = r.Title.Trim();
            var newBody = r.Body.Trim();

            if (string.IsNullOrWhiteSpace(newTitle)) throw new InvalidOperationException("Title is required.");
            if (string.IsNullOrWhiteSpace(newBody)) throw new InvalidOperationException("Body is required.");

            if (r.CategoryId.HasValue)
            {
                var ok = await _db.Categories.AsNoTracking().AnyAsync(c => c.Id == r.CategoryId.Value, ct);
                if (!ok) throw new InvalidOperationException("Category not found.");
            }

            var beforeTitle = post.Title;
            var beforeBody = post.Body;

            post.Title = newTitle;
            post.Body = newBody;
            post.CategoryId = r.CategoryId;
            post.UpdatedAt = DateTime.UtcNow;
            post.UpdatedByUserId = uid;

            // Replace tags
            var tagIds = (r.TagIds ?? new List<Guid>()).Distinct().ToList();
            if (tagIds.Count > 0)
            {
                var existingTagIds = await _db.Tags.AsNoTracking()
                    .Where(t => tagIds.Contains(t.Id))
                    .Select(t => t.Id)
                    .ToListAsync(ct);

                if (existingTagIds.Count != tagIds.Count)
                    throw new InvalidOperationException("One or more tags not found.");
            }

            var oldPostTags = await _db.PostTags.Where(pt => pt.PostId == post.Id).ToListAsync(ct);
            if (oldPostTags.Count > 0) _db.PostTags.RemoveRange(oldPostTags);

            foreach (var tagId in tagIds)
                _db.PostTags.Add(new PostTag { PostId = post.Id, TagId = tagId });

            // Revision record
            _db.PostRevisions.Add(new PostRevision
            {
                PostId = post.Id,
                BeforeTitle = beforeTitle,
                AfterTitle = newTitle,
                BeforeBody = beforeBody,
                AfterBody = newBody,
                Summary = string.IsNullOrWhiteSpace(r.Summary) ? null : r.Summary.Trim(),
                EditedByUserId = uid,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
