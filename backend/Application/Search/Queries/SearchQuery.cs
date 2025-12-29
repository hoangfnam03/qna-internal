using Application.Search.DTOs;
using MediatR;

namespace Application.Search.Queries
{
    public record SearchQuery(
        string Q,
        int Page = 1,
        int PageSize = 20,
        int UsersTake = 10,
        int TagsTake = 10
    ) : IRequest<SearchResponseDto>;
}
