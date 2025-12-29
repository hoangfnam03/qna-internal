using Application.Admin.Users.DTOs;
using Application.Common.Interfaces;
using Domain.Identity.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Users.Commands.AdminSetUserRoles
{
    public class AdminSetUserRolesCommandHandler : IRequestHandler<AdminSetUserRolesCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public AdminSetUserRolesCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db; _current = current;
        }

        public async Task<bool> Handle(AdminSetUserRolesCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var user = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == request.UserId, ct);
            if (user == null) throw new InvalidOperationException("User not found.");
            if (user.IsDeleted) throw new InvalidOperationException("User is deleted.");

            var roleNames = (request.Request.Roles ?? new())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Ensure roles exist (tạo nếu chưa có để demo nhanh)
            var normalized = roleNames.Select(Normalize).ToList();

            var existingRoles = await _db.Roles
                .Where(r => normalized.Contains(r.NormalizedName))
                .ToListAsync(ct);

            var needCreate = roleNames
                .Where(n => existingRoles.All(r => !string.Equals(r.NormalizedName, Normalize(n), StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var name in needCreate)
            {
                _db.Roles.Add(new Role
                {
                    Name = name,
                    NormalizedName = Normalize(name)
                });
            }

            if (needCreate.Count > 0)
                await _db.SaveChangesAsync(ct);

            var allRoles = await _db.Roles
                .Where(r => normalized.Contains(r.NormalizedName))
                .ToListAsync(ct);

            // replace user roles
            var current = await _db.UserRoles.Where(ur => ur.UserId == user.Id).ToListAsync(ct);
            _db.UserRoles.RemoveRange(current);

            var newLinks = allRoles.Select(r => new UserRole { UserId = user.Id, RoleId = r.Id }).ToList();
            _db.UserRoles.AddRange(newLinks);

            await _db.SaveChangesAsync(ct);
            return true;
        }

        private static string Normalize(string s) => s.Trim().ToUpperInvariant();
    }
}
