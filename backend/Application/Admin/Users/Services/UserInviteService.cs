using Application.Admin.Users.DTOs;
using Application.Common.Interfaces;
using Application.Common.Utils;
using Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Common.Options;
using Microsoft.Extensions.Options;

namespace Application.Admin.Users.Services
{
    public class UserInviteService : IUserInviteService
    {
        private readonly IApplicationDbContext _db;
        private readonly IEmailSender _email;
        private readonly InviteOptions _opt;

        public UserInviteService(IApplicationDbContext db, IEmailSender email, IOptions<InviteOptions> opt)
        {
            _db = db;
            _email = email;
            _opt = opt.Value;
        }

        public async Task<InviteIssueResult> IssueInviteAsync(Guid userId, string email, Guid createdByUserId, CancellationToken ct)
        {
            email = email.Trim();
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("Email is required.");

            // expire config (giữ default 24h giống code bạn)
            var minutes = _opt.ExpiresMinutes;
            if (minutes <= 0) minutes = 1440; // 24h

            var rawToken = TokenUtils.NewRawToken();
            var tokenHash = TokenUtils.Sha256Hex(rawToken);
            var exp = DateTime.UtcNow.AddHours(minutes);

            // Upsert theo UserId (token cũ tự vô hiệu)
            var invite = await _db.UserInvites.FirstOrDefaultAsync(x => x.UserId == userId, ct);
            if (invite == null)
            {
                invite = new UserInvite
                {
                    UserId = userId,
                    TokenHash = tokenHash,
                    ExpiresAt = exp,
                    UsedAt = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = createdByUserId,
                    SentToEmail = email
                };
                _db.UserInvites.Add(invite);
            }
            else
            {
                invite.TokenHash = tokenHash;
                invite.ExpiresAt = exp;
                invite.UsedAt = null;
                invite.CreatedAt = DateTime.UtcNow;
                invite.CreatedByUserId = createdByUserId;
                invite.SentToEmail = email;
            }

            await _db.SaveChangesAsync(ct);

            // Mailpit body: giữ y như bạn đang demo
            var subject = "[QnA] Set your password";
            var body = $@"
                <p>You have been invited to QnA system.</p>
                <p><b>Invite token:</b> {rawToken}</p>
                <p>Use this token in API: <code>/api/v1/auth/invite/set-password</code></p>
                <p>Expires at (UTC): {exp:O}</p>
            ";

            await _email.SendAsync(email, subject, body, ct);

            return new InviteIssueResult(userId, email, rawToken, exp);
        }
    }
}
