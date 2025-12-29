using MediatR;

namespace Application.Admin.Users.Commands.AdminDeleteUser
{
    public record AdminDeleteUserCommand(Guid UserId) : IRequest<bool>;
}
