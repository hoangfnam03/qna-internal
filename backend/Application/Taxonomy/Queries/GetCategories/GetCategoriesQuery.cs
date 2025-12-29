using Application.Taxonomy.DTOs;
using MediatR;

namespace Application.Taxonomy.Queries.GetCategories
{
    public record GetCategoriesQuery(bool IncludeHidden = false) : IRequest<List<CategoryDto>>;
}
