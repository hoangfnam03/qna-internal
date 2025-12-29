using Application.Announcements.Queries.GetAnnouncements;
using Application.Announcements.Queries.GetAnnouncementById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/announcements")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AnnouncementsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetAnnouncementsQuery(page, pageSize), ct));

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => Ok(await _mediator.Send(new GetAnnouncementByIdQuery(id, IncludeUnpublished: false), ct));
    }
}
