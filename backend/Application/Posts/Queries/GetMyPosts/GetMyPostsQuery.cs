using Application.Common.Models;
using Application.Posts.DTOs;
using MediatR;

namespace Application.Posts.Queries.GetMyPosts
{
    public record GetMyPostsQuery(int Page = 1, int PageSize = 20) : IRequest<Paged<PostFeedItemDto>>;
}
