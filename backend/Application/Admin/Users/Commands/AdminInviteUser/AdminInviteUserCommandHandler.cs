using Application.Admin.Users.DTOs;
using Application.Auth.DTOs;
using Application.Common.Interfaces;
using Domain.Identity.Entities;
using Domain.Identity.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Users.Commands.AdminInviteUser
{
    public class AdminInviteUserCommandHandler : IRequestHandler<AdminInviteUserCommand, InviteUserResponse>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;
        private readonly IUserInviteService _inviteService;

        public AdminInviteUserCommandHandler(
            IApplicationDbContext db,
            ICurrentUserService current,
            IUserInviteService inviteService)
        {
            _db = db;
            _current = current;
            _inviteService = inviteService;
        }

        public async Task<InviteUserResponse> Handle(AdminInviteUserCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            var adminId = _current.UserId.Value;

            var r = request.Request;
            var email = r.Email.Trim();
            var normEmail = email.ToUpperInvariant();

            // Nếu email đã tồn tại: nếu user đang Invited thì coi như resend (tiện cho admin)
            var existing = await _db.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normEmail, ct);

            User user;
            if (existing != null)
            {
                if (existing.IsDeleted) throw new InvalidOperationException("User is deleted.");
                if (existing.Status != UserStatus.Invited)
                    throw new InvalidOperationException("Email already exists.");

                user = existing;
            }
            else
            {
                var userName = string.IsNullOrWhiteSpace(r.UserName)
                    ? email.Split('@')[0]
                    : r.UserName.Trim();

                // (optional) check username unique
                var normUserName = userName.ToUpperInvariant();
                var userNameExists = await _db.Users.AsNoTracking()
                    .AnyAsync(u => u.NormalizedUserName == normUserName, ct);
                if (userNameExists) throw new InvalidOperationException("UserName already exists.");

                user = new User
                {
                    UserName = userName,
                    NormalizedUserName = normUserName,
                    Email = email,
                    NormalizedEmail = normEmail,
                    EmailConfirmed = false,
                    DisplayName = r.DisplayName.Trim(),
                    Status = UserStatus.Invited
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync(ct);
            }

            // roles (optional) - normalize theo NormalizedName để chắc chắn
            if (r.Roles is { Count: > 0 })
            {
                var normRoles = r.Roles
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .Select(x => x.ToUpperInvariant())
                    .Distinct()
                    .ToList();

                var roles = await _db.Roles
                    .Where(x => normRoles.Contains(x.NormalizedName))
                    .ToListAsync(ct);

                // Replace roles cho sạch (tránh add trùng)
                var currentRoles = await _db.UserRoles.Where(ur => ur.UserId == user.Id).ToListAsync(ct);
                _db.UserRoles.RemoveRange(currentRoles);

                foreach (var role in roles)
                    _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });

                await _db.SaveChangesAsync(ct);
            }

            // ✅ dùng service chung
            var invite = await _inviteService.IssueInviteAsync(user.Id, email, adminId, ct);

            // Trả token để test Swagger ngay (giữ đúng response bạn đang dùng)
            return new InviteUserResponse(invite.UserId, invite.Email, invite.RawToken, invite.ExpiresAt);
        }
    }
}
