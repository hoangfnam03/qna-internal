using Application.Taxonomy.Commands.CreateTag;
using Application.Taxonomy.Commands.DeleteTag;
using Application.Taxonomy.Commands.UpdateTag;
using Application.Taxonomy.DTOs;
using Application.Taxonomy.Queries.GetTags;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/tags")]
    [Authorize(Roles = "Admin")]
    public class AdminTagsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminTagsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetTagsQuery(search, page, pageSize), ct));

        [HttpPost]
        public async Task<IActionResult> Create(CreateTagRequest req, CancellationToken ct)
            => Ok(await _mediator.Send(new CreateTagCommand(req), ct));

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTagRequest req, CancellationToken ct)
            => Ok(await _mediator.Send(new UpdateTagCommand(id, req), ct));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
            => Ok(new { ok = await _mediator.Send(new DeleteTagCommand(id), ct) });
    }
}
