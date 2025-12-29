using Application.Admin.Users.DTOs;
using Application.Common.Interfaces;
using Domain.Identity.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Users.Commands.AdminSuspendUser
{
    public class AdminSuspendUserCommandHandler : IRequestHandler<AdminSuspendUserCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public AdminSuspendUserCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db; _current = current;
        }

        public async Task<bool> Handle(AdminSuspendUserCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var user = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == request.UserId, ct);
            if (user == null) throw new InvalidOperationException("User not found.");
            if (user.IsDeleted) throw new InvalidOperationException("User is deleted.");

            user.Status = UserStatus.Suspended;
            user.SuspendedUntil = request.Request.SuspendedUntil;
            user.SuspensionReason = string.IsNullOrWhiteSpace(request.Request.Reason) ? null : request.Request.Reason.Trim();
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedByUserId = _current.UserId;

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
