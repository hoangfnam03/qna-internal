using Application.Comments.DTOs;
using MediatR;

namespace Application.Comments.Commands.UpdateComment
{
    public record UpdateCommentCommand(Guid CommentId, UpdateCommentRequest Request, bool IsAdmin) : IRequest<bool>;
}
