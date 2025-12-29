using Application.Reports.DTOs;
using MediatR;

namespace Application.Reports.Commands.CreateReport
{
    public record CreateReportCommand(CreateReportRequest Request) : IRequest<Guid>;
}
