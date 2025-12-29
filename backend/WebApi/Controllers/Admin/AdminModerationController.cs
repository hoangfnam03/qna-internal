using Application.Moderation.Commands.AdminDeleteTarget;
using Application.Moderation.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/moderation")]
    [Authorize(Roles = "Admin")]
    public class AdminModerationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminModerationController(IMediator mediator) => _mediator = mediator;

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteTargetRequest req, CancellationToken ct)
            => Ok(new { ok = await _mediator.Send(new AdminDeleteTargetCommand(req), ct) });
    }
}
