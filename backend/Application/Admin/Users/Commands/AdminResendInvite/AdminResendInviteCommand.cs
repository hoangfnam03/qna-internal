using Application.Auth.DTOs;
using Application.Admin.Users.DTOs;
using MediatR;

namespace Application.Admin.Users.Commands.AdminResendInvite
{
    public record AdminResendInviteCommand(Guid UserId, AdminResendInviteRequest Request)
        : IRequest<InviteUserResponse>;
}
