using Application.Auth.DTOs;
using Application.Common.Interfaces;
using Application.Common.Utils;
using Domain.Identity.Entities;
using Domain.Identity.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IApplicationDbContext _db;
        private readonly IPasswordHasherService _hasher;
        private readonly IJwtTokenGenerator _jwt;

        public LoginCommandHandler(IApplicationDbContext db, IPasswordHasherService hasher, IJwtTokenGenerator jwt)
        {
            _db = db;
            _hasher = hasher;
            _jwt = jwt;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken ct)
        {
            var email = request.Request.Email.Trim();
            var normEmail = email.ToUpperInvariant();

            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.NormalizedEmail == normEmail, ct);

            if (user == null) throw new InvalidOperationException("Invalid credentials.");
            if (user.Status != UserStatus.Active) throw new InvalidOperationException("User is not active.");
            if (string.IsNullOrWhiteSpace(user.PasswordHash)) throw new InvalidOperationException("Password not set.");

            if (!_hasher.Verify(request.Request.Password, user.PasswordHash))
                throw new InvalidOperationException("Invalid credentials.");

            // roles
            var roles = await (from ur in _db.UserRoles
                               join r in _db.Roles on ur.RoleId equals r.Id
                               where ur.UserId == user.Id
                               select r.Name).ToListAsync(ct);

            var (access, exp) = _jwt.Generate(user.Id, user.Email, roles);

            // refresh token
            var rawRefresh = TokenUtils.NewRawToken(48);
            var refreshHash = TokenUtils.Sha256Hex(rawRefresh);

            var rt = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshHash,
                ExpiresAt = DateTime.UtcNow.AddDays(14),
                CreatedAt = DateTime.UtcNow,
            };

            _db.RefreshTokens.Add(rt);
            await _db.SaveChangesAsync(ct);

            return new AuthResponse(access, exp, rawRefresh);
        }
    }
}
