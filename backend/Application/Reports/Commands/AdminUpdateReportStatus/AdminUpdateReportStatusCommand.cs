using Application.Reports.DTOs;
using MediatR;

namespace Application.Reports.Commands.AdminUpdateReportStatus
{
    public record AdminUpdateReportStatusCommand(Guid Id, UpdateReportStatusRequest Request) : IRequest<bool>;
}
