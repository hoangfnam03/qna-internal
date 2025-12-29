using Application.Admin.Users.DTOs;
using MediatR;

namespace Application.Admin.Users.Commands.AdminSuspendUser
{
    public record AdminSuspendUserCommand(Guid UserId, AdminUserSuspendRequest Request) : IRequest<bool>;
}
