using Application.Auth.DTOs;
using Application.Common.Interfaces;
using Application.Common.Utils;
using Domain.Identity.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
    {
        private readonly IApplicationDbContext _db;
        private readonly IEmailSender _email;

        public ForgotPasswordCommandHandler(IApplicationDbContext db, IEmailSender email)
        {
            _db = db;
            _email = email;
        }

        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken ct)
        {
            var email = request.Request.Email.Trim();
            var normEmail = email.ToUpperInvariant();

            var user = await _db.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == normEmail, ct);

            // chống enumeration: luôn trả OK
            if (user == null)
                return new ForgotPasswordResponse("If the email exists, a reset token has been sent.", null, null);

            var raw = TokenUtils.NewRawToken(48);
            var hash = TokenUtils.Sha256Hex(raw);
            var exp = DateTime.UtcNow.AddMinutes(30);

            _db.PasswordResetTokens.Add(new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = hash,
                ExpiresAt = exp,
                CreatedAt = DateTime.UtcNow,
                SentToEmail = email
            });

            await _db.SaveChangesAsync(ct);

            var subject = "[QnA] Reset password";
            var body = $@"
                <p>Password reset requested.</p>
                <p><b>Reset token:</b> {raw}</p>
                <p>Use this token in API: <code>/api/v1/auth/reset-password</code></p>
                <p>Expires at (UTC): {exp:O}</p>
            ";
            await _email.SendAsync(email, subject, body, ct);

            // trả token để test Swagger (demo)
            return new ForgotPasswordResponse("Reset token sent (dev).", raw, exp);
        }
    }
}
