using Application.Common.Models;
using Application.Taxonomy.DTOs;
using MediatR;

namespace Application.Taxonomy.Queries.GetTags
{
    public record GetTagsQuery(string? Search, int Page = 1, int PageSize = 20) : IRequest<Paged<TagDto>>;
}
