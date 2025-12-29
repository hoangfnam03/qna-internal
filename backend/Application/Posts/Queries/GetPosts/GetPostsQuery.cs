using Application.Common.Models;
using Application.Posts.DTOs;
using MediatR;

namespace Application.Posts.Queries.GetPosts
{
    public record GetPostsQuery(
        string? Search,
        Guid? CategoryId,
        Guid? TagId,
        Guid? AuthorId,
        string Sort = "recent",
        int Page = 1,
        int PageSize = 20
    ) : IRequest<Paged<PostListItemDto>>;
}
