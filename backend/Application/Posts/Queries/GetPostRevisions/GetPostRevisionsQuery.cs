using Application.Common.Models;
using Application.Posts.DTOs;
using MediatR;

namespace Application.Posts.Queries.GetPostRevisions
{
    public record GetPostRevisionsQuery(Guid PostId, int Page = 1, int PageSize = 50) : IRequest<Paged<PostRevisionDto>>;
}
