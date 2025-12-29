using Application.Taxonomy.DTOs;
using MediatR;

namespace Application.Taxonomy.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(Guid Id, UpdateCategoryRequest Request) : IRequest<CategoryDto>;
}
