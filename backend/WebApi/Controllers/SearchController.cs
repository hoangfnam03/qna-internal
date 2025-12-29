using Application.Search.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/search")]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SearchController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string q,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int usersTake = 10,
            [FromQuery] int tagsTake = 10,
            CancellationToken ct = default)
            => Ok(await _mediator.Send(new SearchQuery(q, page, pageSize, usersTake, tagsTake), ct));
    }
}
