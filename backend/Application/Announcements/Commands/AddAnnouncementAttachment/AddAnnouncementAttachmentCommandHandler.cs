using Application.Common.Interfaces;
using Domain.Communication.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Announcements.Commands.AddAnnouncementAttachment
{
    public class AddAnnouncementAttachmentCommandHandler : IRequestHandler<AddAnnouncementAttachmentCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        public AddAnnouncementAttachmentCommandHandler(IApplicationDbContext db) => _db = db;

        public async Task<bool> Handle(AddAnnouncementAttachmentCommand request, CancellationToken ct)
        {
            var aExists = await _db.Announcements.AsNoTracking().AnyAsync(x => x.Id == request.AnnouncementId, ct);
            if (!aExists) throw new InvalidOperationException("Announcement not found.");

            var fExists = await _db.Files.AsNoTracking().AnyAsync(x => x.Id == request.FileId, ct);
            if (!fExists) throw new InvalidOperationException("File not found.");

            var exists = await _db.AnnouncementAttachments.AsNoTracking()
                .AnyAsync(x => x.AnnouncementId == request.AnnouncementId && x.FileId == request.FileId, ct);

            if (!exists)
            {
                _db.AnnouncementAttachments.Add(new AnnouncementAttachment
                {
                    AnnouncementId = request.AnnouncementId,
                    FileId = request.FileId
                });
                await _db.SaveChangesAsync(ct);
            }

            return true;
        }
    }
}
