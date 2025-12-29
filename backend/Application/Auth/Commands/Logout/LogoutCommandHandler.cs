using Application.Common.Interfaces;
using Application.Common.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IApplicationDbContext _db;

        public LogoutCommandHandler(IApplicationDbContext db) { _db = db; }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken ct)
        {
            var raw = request.Request.RefreshToken.Trim();
            var hash = TokenUtils.Sha256Hex(raw);

            var token = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash, ct);
            if (token == null) return true;

            token.RevokedAt ??= DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
