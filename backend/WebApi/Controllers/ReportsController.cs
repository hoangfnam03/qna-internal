using Application.Reports.Commands.CreateReport;
using Application.Reports.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ReportsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create(CreateReportRequest req, CancellationToken ct)
            => Ok(new { id = await _mediator.Send(new CreateReportCommand(req), ct) });
    }
}
