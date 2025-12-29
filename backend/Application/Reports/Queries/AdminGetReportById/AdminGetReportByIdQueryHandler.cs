using Application.Common.Interfaces;
using Application.Reports.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Reports.Queries.AdminGetReportById
{
    public class AdminGetReportByIdQueryHandler : IRequestHandler<AdminGetReportByIdQuery, ReportDetailDto>
    {
        private readonly IApplicationDbContext _db;
        public AdminGetReportByIdQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<ReportDetailDto> Handle(AdminGetReportByIdQuery request, CancellationToken ct)
        {
            var r = await _db.Reports.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (r == null) throw new InvalidOperationException("Report not found.");

            var reporterName = await _db.Users.IgnoreQueryFilters().AsNoTracking()
                .Where(u => u.Id == r.ReporterUserId)
                .Select(u => u.DisplayName)
                .FirstOrDefaultAsync(ct) ?? "Unknown";

            string? reviewerName = null;
            if (r.ReviewedByUserId.HasValue)
            {
                reviewerName = await _db.Users.AsNoTracking()
                    .Where(u => u.Id == r.ReviewedByUserId.Value)
                    .Select(u => u.DisplayName)
                    .FirstOrDefaultAsync(ct);
            }

            return new ReportDetailDto(
                r.Id,
                r.Status,
                r.TargetType,
                r.TargetId,
                r.ReasonCode,
                r.ReasonText,
                r.EvidenceJson,
                r.CreatedAt,
                r.ReporterUserId,
                reporterName,
                r.ReviewedAt,
                r.ReviewedByUserId,
                reviewerName,
                r.ResolutionNote
            );
        }
    }
}
