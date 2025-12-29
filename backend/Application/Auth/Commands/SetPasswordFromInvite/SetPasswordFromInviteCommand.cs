using Application.Auth.DTOs;
using MediatR;

namespace Application.Auth.Commands.SetPasswordFromInvite
{
    public record SetPasswordFromInviteCommand(SetPasswordFromInviteRequest Request) : IRequest<bool>;
}
