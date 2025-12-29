using Application.Attachments.DTOs;
using Application.Common.Interfaces;
using Domain.Content.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Attachments.Commands.AddPostAttachment
{
    public class AddPostAttachmentCommandHandler : IRequestHandler<AddPostAttachmentCommand, Guid>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public AddPostAttachmentCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db; _current = current;
        }

        public async Task<Guid> Handle(AddPostAttachmentCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            var uid = _current.UserId.Value;

            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId, ct);
            if (post == null) throw new InvalidOperationException("Post not found.");

            if (!(request.IsAdmin || post.CreatedByUserId == uid))
                throw new UnauthorizedAccessException("You cannot attach files to this post.");

            var fileExists = await _db.Files.AsNoTracking().AnyAsync(f => f.Id == request.Request.FileId, ct);
            if (!fileExists) throw new InvalidOperationException("File not found.");

            var att = new PostAttachment
            {
                PostId = post.Id,
                FileId = request.Request.FileId,
                Caption = request.Request.Caption,
                DisplayText = request.Request.DisplayText,
                SortOrder = request.Request.SortOrder,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = uid
            };

            _db.PostAttachments.Add(att);
            await _db.SaveChangesAsync(ct);
            return att.Id;
        }
    }
}
