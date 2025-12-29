using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Reports.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Reports.Queries.AdminGetReports
{
    public class AdminGetReportsQueryHandler : IRequestHandler<AdminGetReportsQuery, Paged<ReportListItemDto>>
    {
        private readonly IApplicationDbContext _db;
        public AdminGetReportsQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<Paged<ReportListItemDto>> Handle(AdminGetReportsQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;

            var q = _db.Reports.AsNoTracking();

            if (request.Status.HasValue) q = q.Where(x => x.Status == request.Status.Value);
            if (request.TargetType.HasValue) q = q.Where(x => x.TargetType == request.TargetType.Value);

            q = q.OrderByDescending(x => x.CreatedAt);

            var total = await q.CountAsync(ct);

            var items = await (
                from r in q.Skip((page - 1) * pageSize).Take(pageSize)
                join u in _db.Users.IgnoreQueryFilters().AsNoTracking()
                    on r.ReporterUserId equals u.Id

                select new ReportListItemDto(
                    r.Id,
                    r.Status,
                    r.TargetType,
                    r.TargetId,
                    r.ReasonCode,
                    r.CreatedAt,
                    r.ReporterUserId,
                    u.DisplayName
                )
            ).ToListAsync(ct);

            return new Paged<ReportListItemDto>(items, page, pageSize, total);
        }
    }
}
