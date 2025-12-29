using Application.Common.Interfaces;
using Application.Common.Utils;
using Domain.Identity.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands.SetPasswordFromInvite
{
    public class SetPasswordFromInviteCommandHandler : IRequestHandler<SetPasswordFromInviteCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly IPasswordHasherService _hasher;

        public SetPasswordFromInviteCommandHandler(IApplicationDbContext db, IPasswordHasherService hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        public async Task<bool> Handle(SetPasswordFromInviteCommand request, CancellationToken ct)
        {
            var raw = request.Request.InviteToken.Trim();
            var hash = TokenUtils.Sha256Hex(raw);

            var invite = await _db.UserInvites
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == hash, ct);

            if (invite == null) throw new InvalidOperationException("Invalid invite token.");
            if (invite.UsedAt != null) throw new InvalidOperationException("Invite token already used.");
            if (invite.ExpiresAt <= DateTime.UtcNow) throw new InvalidOperationException("Invite token expired.");

            var user = invite.User;

            user.PasswordHash = _hasher.Hash(request.Request.NewPassword);
            user.Status = UserStatus.Active;
            user.EmailConfirmed = true;

            invite.UsedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
