using Application.Reports.Commands.AdminUpdateReportStatus;
using Application.Reports.DTOs;
using Application.Reports.Queries.AdminGetReportById;
using Application.Reports.Queries.AdminGetReports;
using Domain.Moderation.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/reports")]
    [Authorize(Roles = "Admin")]
    public class AdminReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminReportsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] ReportStatus? status,
            [FromQuery] ReportTargetType? targetType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
            => Ok(await _mediator.Send(new AdminGetReportsQuery(status, targetType, page, pageSize), ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => Ok(await _mediator.Send(new AdminGetReportByIdQuery(id), ct));

        [HttpPost("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateReportStatusRequest req, CancellationToken ct)
            => Ok(new { ok = await _mediator.Send(new AdminUpdateReportStatusCommand(id, req), ct) });
    }
}
