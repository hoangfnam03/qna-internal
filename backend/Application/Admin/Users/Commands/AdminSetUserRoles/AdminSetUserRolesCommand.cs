using Application.Admin.Users.DTOs;
using MediatR;

namespace Application.Admin.Users.Commands.AdminSetUserRoles
{
    public record AdminSetUserRolesCommand(Guid UserId, AdminSetUserRolesRequest Request) : IRequest<bool>;
}
