using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;
        private readonly IPasswordHasherService _hasher;

        public ChangePasswordCommandHandler(IApplicationDbContext db, ICurrentUserService current, IPasswordHasherService hasher)
        {
            _db = db;
            _current = current;
            _hasher = hasher;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var user = await _db.Users.FirstAsync(x => x.Id == _current.UserId.Value, ct);
            if (string.IsNullOrWhiteSpace(user.PasswordHash)) throw new InvalidOperationException("Password not set.");

            if (!_hasher.Verify(request.Request.OldPassword, user.PasswordHash))
                throw new InvalidOperationException("Old password is incorrect.");

            user.PasswordHash = _hasher.Hash(request.Request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedByUserId = user.Id;

            // revoke all refresh tokens of this user (recommended)
            var tokens = await _db.RefreshTokens.Where(x => x.UserId == user.Id && x.RevokedAt == null).ToListAsync(ct);
            foreach (var t in tokens) t.RevokedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
