using Application.Posts.DTOs;
using MediatR;

namespace Application.Posts.Commands.UpdatePost
{
    public record UpdatePostCommand(Guid Id, UpdatePostRequest Request, bool IsAdmin) : IRequest<bool>;
}
