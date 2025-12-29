using Application.Common.Models;
using Application.Comments.DTOs;
using MediatR;

namespace Application.Comments.Queries.GetCommentRevisions
{
    public record GetCommentRevisionsQuery(Guid CommentId, int Page = 1, int PageSize = 50) : IRequest<Paged<CommentRevisionDto>>;
}
