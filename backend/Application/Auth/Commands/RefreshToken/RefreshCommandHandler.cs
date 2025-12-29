using Application.Auth.DTOs;
using Application.Common.Interfaces;
using Application.Common.Utils;
using Domain.Identity.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthResponse>
    {
        private readonly IApplicationDbContext _db;
        private readonly IJwtTokenGenerator _jwt;

        public RefreshCommandHandler(IApplicationDbContext db, IJwtTokenGenerator jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<AuthResponse> Handle(RefreshCommand request, CancellationToken ct)
        {
            var raw = request.Request.RefreshToken.Trim();
            var hash = TokenUtils.Sha256Hex(raw);

            var token = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash, ct);
            if (token == null) throw new InvalidOperationException("Invalid refresh token.");
            if (token.RevokedAt != null) throw new InvalidOperationException("Refresh token revoked.");
            if (token.ExpiresAt <= DateTime.UtcNow) throw new InvalidOperationException("Refresh token expired.");

            // issue new access
            var user = await _db.Users.FirstAsync(x => x.Id == token.UserId, ct);

            var roles = await (from ur in _db.UserRoles
                               join r in _db.Roles on ur.RoleId equals r.Id
                               where ur.UserId == user.Id
                               select r.Name).ToListAsync(ct);

            var (access, exp) = _jwt.Generate(user.Id, user.Email, roles);

            // rotate refresh
            var newRaw = TokenUtils.NewRawToken(48);
            var newHash = TokenUtils.Sha256Hex(newRaw);

            token.RevokedAt = DateTime.UtcNow;
            token.ReplacedByTokenHash = newHash;

            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newHash,
                ExpiresAt = DateTime.UtcNow.AddDays(14),
                CreatedAt = DateTime.UtcNow,
            });

            await _db.SaveChangesAsync(ct);

            return new AuthResponse(access, exp, newRaw);
        }
    }
}
