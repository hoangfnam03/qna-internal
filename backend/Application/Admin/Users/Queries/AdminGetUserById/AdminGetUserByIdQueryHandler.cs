using Application.Admin.Users.DTOs;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Users.Queries.AdminGetUserById
{
    public class AdminGetUserByIdQueryHandler : IRequestHandler<AdminGetUserByIdQuery, AdminUserDetailDto>
    {
        private readonly IApplicationDbContext _db;
        public AdminGetUserByIdQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<AdminUserDetailDto> Handle(AdminGetUserByIdQuery request, CancellationToken ct)
        {
            var q = _db.Users.AsNoTracking();
            if (request.IncludeDeleted) q = q.IgnoreQueryFilters();

            var u = await q.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (u == null) throw new InvalidOperationException("User not found.");

            var roles = await (
                from ur in _db.UserRoles.AsNoTracking()
                join r in _db.Roles.AsNoTracking() on ur.RoleId equals r.Id
                where ur.UserId == u.Id
                select r.Name
            ).Distinct().OrderBy(x => x).ToListAsync(ct);

            return new AdminUserDetailDto(
                u.Id,
                u.UserName,
                u.DisplayName,
                u.Email,
                u.EmailConfirmed,
                u.Status,
                u.SuspendedUntil,
                u.SuspensionReason,
                u.Bio,
                u.AvatarUrl,
                u.CreatedAt,
                u.UpdatedAt,
                u.DeletedAt,
                u.IsDeleted,
                roles
            );
        }
    }
}
