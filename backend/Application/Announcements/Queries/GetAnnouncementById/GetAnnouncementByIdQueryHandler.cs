using Application.Announcements.DTOs;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Announcements.Queries.GetAnnouncementById
{
    public class GetAnnouncementByIdQueryHandler : IRequestHandler<GetAnnouncementByIdQuery, AnnouncementDetailDto>
    {
        private readonly IApplicationDbContext _db;
        public GetAnnouncementByIdQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<AnnouncementDetailDto> Handle(GetAnnouncementByIdQuery request, CancellationToken ct)
        {
            var a = await _db.Announcements.AsNoTracking()
                .Where(x => x.Id == request.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Body,
                    x.IsPublished,
                    x.PublishedAt,
                    x.CreatedAt,
                    x.UpdatedAt,
                    x.CreatedByUserId
                })
                .FirstOrDefaultAsync(ct);

            if (a == null) throw new InvalidOperationException("Announcement not found.");
            if (!request.IncludeUnpublished && !a.IsPublished) throw new InvalidOperationException("Announcement not published.");

            var createdByName = await _db.Users.AsNoTracking()
                .Where(u => u.Id == a.CreatedByUserId)
                .Select(u => u.DisplayName)
                .FirstOrDefaultAsync(ct) ?? "Unknown";

            var attachments = await (
                from aa in _db.AnnouncementAttachments.AsNoTracking()
                join f in _db.Files.AsNoTracking() on aa.FileId equals f.Id
                where aa.AnnouncementId == a.Id
                orderby f.UploadedAt descending
                select new AnnouncementAttachmentDto(
                    f.Id,
                    f.OriginalFileName,
                    f.ContentType,
                    f.SizeBytes,
                    // Public URL: đúng với LocalFileStorageOptions.PublicBaseUrl="/storage"
                    "/storage/" + f.StoragePath
                )
            ).ToListAsync(ct);

            return new AnnouncementDetailDto(
                a.Id,
                a.Title,
                a.Body,
                a.IsPublished,
                a.PublishedAt,
                a.CreatedAt,
                a.UpdatedAt,
                createdByName,
                attachments
            );
        }
    }
}
