using Application.Attachments.Commands.AddPostAttachment;
using Application.Attachments.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/posts/{postId:guid}/attachments")]
    [Authorize]
    public class PostAttachmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostAttachmentsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Add(Guid postId, AddAttachmentRequest req, CancellationToken ct)
        {
            var isAdmin = User.IsInRole("Admin");
            var id = await _mediator.Send(new AddPostAttachmentCommand(postId, req, isAdmin), ct);
            return Ok(new { id });
        }
    }
}
