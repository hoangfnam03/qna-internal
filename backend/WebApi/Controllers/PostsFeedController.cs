using Application.Posts.Queries.GetMyPosts;
using Application.Posts.Queries.GetPostFeed;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/feed/posts")]
    public class PostsFeedController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostsFeedController(IMediator mediator) => _mediator = mediator;

        // /api/v1/posts?sort=recent|popular&filter=unanswered&page=1&pageSize=20&authorId=...
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? sort,
            [FromQuery] string? filter,
            [FromQuery] Guid? authorId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetPostFeedQuery(sort, filter, authorId, page, pageSize), ct));

        // /api/v1/posts/me
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> MyPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetMyPostsQuery(page, pageSize), ct));
    }
}
