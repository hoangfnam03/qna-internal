using Application.Attachments.Commands.AddCommentAttachment;
using Application.Attachments.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/comments/{commentId:guid}/attachments")]
    [Authorize]
    public class CommentAttachmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CommentAttachmentsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Add(Guid commentId, AddAttachmentRequest req, CancellationToken ct)
        {
            var isAdmin = User.IsInRole("Admin");
            var id = await _mediator.Send(new AddCommentAttachmentCommand(commentId, req, isAdmin), ct);
            return Ok(new { id });
        }
    }
}
