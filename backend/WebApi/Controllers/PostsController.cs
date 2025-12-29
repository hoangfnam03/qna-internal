using Application.Posts.Commands.CreatePost;
using Application.Posts.Commands.UpdatePost;
using Application.Posts.DTOs;
using Application.Posts.Queries.GetPostById;
using Application.Posts.Queries.GetPostRevisions;
using Application.Posts.Queries.GetPosts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/posts")]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostsController(IMediator mediator) => _mediator = mediator;

        // GET /posts?sort=recent|popular|unanswered&search=&categoryId=&tagId=&authorId=&page=&pageSize=
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetList(
            [FromQuery] string? search,
            [FromQuery] Guid? categoryId,
            [FromQuery] Guid? tagId,
            [FromQuery] Guid? authorId,
            [FromQuery] string sort = "recent",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var res = await _mediator.Send(new GetPostsQuery(search, categoryId, tagId, authorId, sort, page, pageSize), ct);
            return Ok(res);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<PostDetailDto>> GetById(Guid id, CancellationToken ct)
            => Ok(await _mediator.Send(new GetPostByIdQuery(id), ct));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreatePostRequest req, CancellationToken ct)
        {
            var id = await _mediator.Send(new CreatePostCommand(req), ct);
            return Ok(new { id });
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, UpdatePostRequest req, CancellationToken ct)
        {
            var isAdmin = User.IsInRole("Admin");
            var ok = await _mediator.Send(new UpdatePostCommand(id, req, isAdmin), ct);
            return Ok(new { ok });
        }

        [HttpGet("{id:guid}/revisions")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRevisions(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetPostRevisionsQuery(id, page, pageSize), ct));
    }
}
