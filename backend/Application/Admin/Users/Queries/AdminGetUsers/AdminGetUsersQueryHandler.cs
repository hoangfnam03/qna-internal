using Application.Admin.Users.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Identity.Entities;
using Domain.Identity.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Users.Queries.AdminGetUsers
{
    public class AdminGetUsersQueryHandler : IRequestHandler<AdminGetUsersQuery, Paged<AdminUserListItemDto>>
    {
        private readonly IApplicationDbContext _db;
        public AdminGetUsersQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<Paged<AdminUserListItemDto>> Handle(AdminGetUsersQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;

            IQueryable<User> q = _db.Users.AsNoTracking();
            if (request.IncludeDeleted) q = q.IgnoreQueryFilters();

            if (request.Status.HasValue)
                q = q.Where(u => u.Status == request.Status.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.Trim().ToLower();
                q = q.Where(u =>
                    u.UserName.ToLower().Contains(s) ||
                    u.DisplayName.ToLower().Contains(s) ||
                    (u.Email != null && u.Email.ToLower().Contains(s))
                );
            }

            q = q.OrderByDescending(u => u.CreatedAt);

            var total = await q.CountAsync(ct);

            var users = await q.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.DisplayName,
                    u.Email,
                    u.Status,
                    u.SuspendedUntil,
                    u.SuspensionReason,
                    u.CreatedAt
                })
                .ToListAsync(ct);

            var ids = users.Select(x => x.Id).ToList();

            // roles
            var roleMap = await (
                from ur in _db.UserRoles.AsNoTracking()
                join r in _db.Roles.AsNoTracking() on ur.RoleId equals r.Id
                where ids.Contains(ur.UserId)
                select new { ur.UserId, RoleName = r.Name }
            ).ToListAsync(ct);

            var rolesByUser = roleMap
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.RoleName).Distinct().OrderBy(x => x).ToList());

            var items = users.Select(u => new AdminUserListItemDto(
                u.Id,
                u.UserName,
                u.DisplayName,
                u.Email,
                u.Status,
                u.SuspendedUntil,
                u.SuspensionReason,
                u.CreatedAt,
                rolesByUser.TryGetValue(u.Id, out var rs) ? rs : new List<string>()
            )).ToList();

            return new Paged<AdminUserListItemDto>(items, page, pageSize, total);
        }
    }
}
