using Application.Announcements.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Announcements.Queries.AdminGetAnnouncements
{
    public class AdminGetAnnouncementsQueryHandler : IRequestHandler<AdminGetAnnouncementsQuery, Paged<AnnouncementListItemDto>>
    {
        private readonly IApplicationDbContext _db;
        public AdminGetAnnouncementsQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<Paged<AnnouncementListItemDto>> Handle(AdminGetAnnouncementsQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;

            var q = _db.Announcements.AsNoTracking();

            if (request.PublishedOnly) q = q.Where(a => a.IsPublished);

            q = q.OrderByDescending(a => a.PublishedAt ?? a.CreatedAt);

            var total = await q.CountAsync(ct);

            var items = await (
                from a in q.Skip((page - 1) * pageSize).Take(pageSize)
                join u in _db.Users.AsNoTracking() on a.CreatedByUserId equals u.Id
                select new AnnouncementListItemDto(
                    a.Id,
                    a.Title,
                    a.IsPublished,
                    a.PublishedAt,
                    a.CreatedAt,
                    u.DisplayName
                )
            ).ToListAsync(ct);

            return new Paged<AnnouncementListItemDto>(items, page, pageSize, total);
        }
    }
}
