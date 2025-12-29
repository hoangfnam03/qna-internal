using Application.Comments.DTOs;
using MediatR;

namespace Application.Comments.Queries.GetCommentsByPost
{
    public record GetCommentsByPostQuery(Guid PostId) : IRequest<List<CommentItemDto>>;
}
