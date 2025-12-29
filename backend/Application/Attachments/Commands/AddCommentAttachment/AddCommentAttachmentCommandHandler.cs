using Application.Attachments.DTOs;
using Application.Common.Interfaces;
using Domain.Content.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Attachments.Commands.AddCommentAttachment
{
    public class AddCommentAttachmentCommandHandler : IRequestHandler<AddCommentAttachmentCommand, Guid>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public AddCommentAttachmentCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db; _current = current;
        }

        public async Task<Guid> Handle(AddCommentAttachmentCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            var uid = _current.UserId.Value;

            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId, ct);
            if (comment == null) throw new InvalidOperationException("Comment not found.");

            if (!(request.IsAdmin || comment.CreatedByUserId == uid))
                throw new UnauthorizedAccessException("You cannot attach files to this comment.");

            var fileExists = await _db.Files.AsNoTracking().AnyAsync(f => f.Id == request.Request.FileId, ct);
            if (!fileExists) throw new InvalidOperationException("File not found.");

            var att = new CommentAttachment
            {
                CommentId = comment.Id,
                FileId = request.Request.FileId,
                Caption = request.Request.Caption,
                DisplayText = request.Request.DisplayText,
                SortOrder = request.Request.SortOrder,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = uid
            };

            _db.CommentAttachments.Add(att);
            await _db.SaveChangesAsync(ct);
            return att.Id;
        }
    }
}
