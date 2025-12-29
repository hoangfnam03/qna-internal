using Application.Taxonomy.DTOs;
using MediatR;

namespace Application.Taxonomy.Commands.CreateCategory
{
    public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryDto>;
}
