using MediatR;

namespace Application.Taxonomy.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid Id) : IRequest<bool>;
}
