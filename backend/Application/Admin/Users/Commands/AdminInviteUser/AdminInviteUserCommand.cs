using Application.Auth.DTOs;
using MediatR;

namespace Application.Admin.Users.Commands.AdminInviteUser
{
    public record AdminInviteUserCommand(InviteUserRequest Request) : IRequest<InviteUserResponse>;
}
