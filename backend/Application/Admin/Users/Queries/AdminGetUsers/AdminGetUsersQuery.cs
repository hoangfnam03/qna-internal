using Application.Admin.Users.DTOs;
using Application.Common.Models;
using Domain.Identity.Enums;
using MediatR;

namespace Application.Admin.Users.Queries.AdminGetUsers
{
    public record AdminGetUsersQuery(
        string? Search,
        UserStatus? Status,
        bool IncludeDeleted = false,
        int Page = 1,
        int PageSize = 50
    ) : IRequest<Paged<AdminUserListItemDto>>;
}
