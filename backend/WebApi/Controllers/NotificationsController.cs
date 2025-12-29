using Application.Notifications.Commands.MarkNotificationRead;
using Application.Notifications.Queries.GetMyNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NotificationsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool unreadOnly = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetMyNotificationsQuery(unreadOnly, page, pageSize), ct));

        [HttpPost("{id:guid}/read")]
        public async Task<IActionResult> Read(Guid id, CancellationToken ct)
            => Ok(new { ok = await _mediator.Send(new MarkNotificationReadCommand(id), ct) });
    }
}
