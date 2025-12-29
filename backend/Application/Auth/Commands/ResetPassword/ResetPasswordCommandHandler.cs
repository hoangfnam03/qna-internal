using Application.Common.Interfaces;
using Application.Common.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly IPasswordHasherService _hasher;

        public ResetPasswordCommandHandler(IApplicationDbContext db, IPasswordHasherService hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            var raw = request.Request.ResetToken.Trim();
            var hash = TokenUtils.Sha256Hex(raw);

            var token = await _db.PasswordResetTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

            if (token == null) throw new InvalidOperationException("Invalid reset token.");
            if (token.UsedAt != null) throw new InvalidOperationException("Reset token already used.");
            if (token.ExpiresAt <= DateTime.UtcNow) throw new InvalidOperationException("Reset token expired.");

            var user = token.User;
            user.PasswordHash = _hasher.Hash(request.Request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedByUserId = user.Id;

            token.UsedAt = DateTime.UtcNow;

            // revoke all refresh tokens
            var refreshes = await _db.RefreshTokens.Where(x => x.UserId == user.Id && x.RevokedAt == null).ToListAsync(ct);
            foreach (var r in refreshes) r.RevokedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
