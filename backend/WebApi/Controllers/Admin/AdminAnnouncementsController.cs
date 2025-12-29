using Application.Announcements.Commands.AddAnnouncementAttachment;
using Application.Announcements.Commands.CreateAnnouncement;
using Application.Announcements.Commands.PublishAnnouncement;
using Application.Announcements.Commands.UpdateAnnouncement;
using Application.Announcements.DTOs;
using Application.Announcements.Queries.AdminGetAnnouncements;
using Application.Announcements.Queries.GetAnnouncementById;
using Application.Attachments.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/announcements")]
    [Authorize(Roles = "Admin")]
    public class AdminAnnouncementsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminAnnouncementsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool publishedOnly = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
            => Ok(await _mediator.Send(new AdminGetAnnouncementsQuery(publishedOnly, page, pageSize), ct));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => Ok(await _mediator.Send(new GetAnnouncementByIdQuery(id, IncludeUnpublished: true), ct));

        [HttpPost]
        public async Task<IActionResult> Create(CreateAnnouncementRequest req, CancellationToken ct)
            => Ok(new { id = await _mediator.Send(new CreateAnnouncementCommand(req), ct) });

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateAnnouncementRequest req, CancellationToken ct)
            => Ok(new { ok = await _mediator.Send(new UpdateAnnouncementCommand(id, req), ct) });

        [HttpPost("{id:guid}/attachments")]
        public async Task<IActionResult> AddAttachment(Guid id, AddAttachmentRequest req, CancellationToken ct)
            => Ok(new { ok = await _mediator.Send(new AddAnnouncementAttachmentCommand(id, req.FileId), ct) });

        [HttpPost("{id:guid}/publish")]
        public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
            => Ok(new { ok = await _mediator.Send(new PublishAnnouncementCommand(id), ct) });
    }
}
