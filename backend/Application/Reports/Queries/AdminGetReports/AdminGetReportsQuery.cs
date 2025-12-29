using Application.Common.Models;
using Application.Reports.DTOs;
using Domain.Moderation.Enums;
using MediatR;

namespace Application.Reports.Queries.AdminGetReports
{
    public record AdminGetReportsQuery(
        ReportStatus? Status,
        ReportTargetType? TargetType,
        int Page = 1,
        int PageSize = 50
    ) : IRequest<Paged<ReportListItemDto>>;
}
