using MediatR;

namespace Application.Admin.Users.Commands.AdminUnsuspendUser
{
    public record AdminUnsuspendUserCommand(Guid UserId) : IRequest<bool>;
}
