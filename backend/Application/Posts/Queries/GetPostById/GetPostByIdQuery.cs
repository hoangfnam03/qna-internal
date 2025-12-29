using Application.Posts.DTOs;
using MediatR;

namespace Application.Posts.Queries.GetPostById
{
    public record GetPostByIdQuery(Guid Id) : IRequest<PostDetailDto>;
}
