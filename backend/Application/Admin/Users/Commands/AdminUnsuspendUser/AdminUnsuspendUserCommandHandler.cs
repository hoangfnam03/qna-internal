using Application.Common.Interfaces;
using Domain.Identity.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Users.Commands.AdminUnsuspendUser
{
    public class AdminUnsuspendUserCommandHandler : IRequestHandler<AdminUnsuspendUserCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public AdminUnsuspendUserCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db; _current = current;
        }

        public async Task<bool> Handle(AdminUnsuspendUserCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var user = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == request.UserId, ct);
            if (user == null) throw new InvalidOperationException("User not found.");
            if (user.IsDeleted) throw new InvalidOperationException("User is deleted.");

            user.Status = UserStatus.Active;
            user.SuspendedUntil = null;
            user.SuspensionReason = null;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedByUserId = _current.UserId;

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
