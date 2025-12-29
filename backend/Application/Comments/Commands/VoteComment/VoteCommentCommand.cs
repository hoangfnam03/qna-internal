using Application.Comments.DTOs;
using MediatR;

namespace Application.Comments.Commands.VoteComment
{
    public record VoteCommentCommand(Guid CommentId, VoteRequest Request) : IRequest<int>; // returns new score
}
