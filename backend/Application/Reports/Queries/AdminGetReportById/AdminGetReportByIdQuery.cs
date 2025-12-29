using Application.Reports.DTOs;
using MediatR;

namespace Application.Reports.Queries.AdminGetReportById
{
    public record AdminGetReportByIdQuery(Guid Id) : IRequest<ReportDetailDto>;
}
