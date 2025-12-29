using Application.Comments.Commands.CreateComment;
using Application.Comments.DTOs;
using Application.Comments.Queries.GetCommentsByPost;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/posts/{postId:guid}/comments")]
    public class PostCommentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostCommentsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Guid postId, CancellationToken ct)
            => Ok(await _mediator.Send(new GetCommentsByPostQuery(postId), ct));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Guid postId, CreateCommentRequest req, CancellationToken ct)
        {
            var id = await _mediator.Send(new CreateCommentCommand(postId, req), ct);
            return Ok(new { id });
        }
    }
}
