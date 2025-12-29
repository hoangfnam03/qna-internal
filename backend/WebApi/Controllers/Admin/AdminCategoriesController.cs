using Application.Taxonomy.Commands.CreateCategory;
using Application.Taxonomy.Commands.DeleteCategory;
using Application.Taxonomy.Commands.UpdateCategory;
using Application.Taxonomy.DTOs;
using Application.Taxonomy.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/categories")]
    [Authorize(Roles = "Admin")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminCategoriesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(await _mediator.Send(new GetCategoriesQuery(true), ct));

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest req, CancellationToken ct)
            => Ok(await _mediator.Send(new CreateCategoryCommand(req), ct));

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCategoryRequest req, CancellationToken ct)
            => Ok(await _mediator.Send(new UpdateCategoryCommand(id, req), ct));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
            => Ok(new { ok = await _mediator.Send(new DeleteCategoryCommand(id), ct) });
    }
}
