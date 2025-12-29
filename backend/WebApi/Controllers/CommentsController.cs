using Application.Comments.Commands.UpdateComment;
using Application.Comments.Commands.VoteComment;
using Application.Comments.DTOs;
using Application.Comments.Queries.GetCommentRevisions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CommentsController(IMediator mediator) => _mediator = mediator;

        [HttpPut("{commentId:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid commentId, UpdateCommentRequest req, CancellationToken ct)
        {
            var isAdmin = User.IsInRole("Admin");
            var ok = await _mediator.Send(new UpdateCommentCommand(commentId, req, isAdmin), ct);
            return Ok(new { ok });
        }

        [HttpGet("{commentId:guid}/revisions")]
        [AllowAnonymous]
        public async Task<IActionResult> Revisions(Guid commentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetCommentRevisionsQuery(commentId, page, pageSize), ct));

        [HttpPost("{commentId:guid}/vote")]
        [Authorize]
        public async Task<IActionResult> Vote(Guid commentId, VoteRequest req, CancellationToken ct)
            => Ok(new { score = await _mediator.Send(new VoteCommentCommand(commentId, req), ct) });
    }
}
