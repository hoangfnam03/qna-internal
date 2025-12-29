using Application.Comments.DTOs;
using MediatR;

namespace Application.Comments.Commands.CreateComment
{
    public record CreateCommentCommand(Guid PostId, CreateCommentRequest Request) : IRequest<Guid>;
}
