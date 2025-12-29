using Application.Announcements.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Announcements.Queries.GetAnnouncements
{
    public class GetAnnouncementsQueryHandler : IRequestHandler<GetAnnouncementsQuery, Paged<AnnouncementListItemDto>>
    {
        private readonly IApplicationDbContext _db;
        public GetAnnouncementsQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<Paged<AnnouncementListItemDto>> Handle(GetAnnouncementsQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 200 ? 20 : request.PageSize;

            var q = _db.Announcements.AsNoTracking()
                .Where(a => a.IsPublished)
                .OrderByDescending(a => a.PublishedAt ?? a.CreatedAt);

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
