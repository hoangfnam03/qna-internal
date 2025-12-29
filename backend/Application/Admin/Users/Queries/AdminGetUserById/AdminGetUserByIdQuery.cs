using Application.Admin.Users.DTOs;
using MediatR;

namespace Application.Admin.Users.Queries.AdminGetUserById
{
    public record AdminGetUserByIdQuery(Guid Id, bool IncludeDeleted = true) : IRequest<AdminUserDetailDto>;
}
