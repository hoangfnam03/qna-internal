using Application.Common.Models;
using Application.Posts.DTOs;
using MediatR;

namespace Application.Posts.Queries.GetPostFeed
{
    // sort: "recent" | "popular"
    // filter: "unanswered" | null
    public record GetPostFeedQuery(
        string? Sort,
        string? Filter,
        Guid? AuthorId,
        int Page = 1,
        int PageSize = 20
    ) : IRequest<Paged<PostFeedItemDto>>;
}
