using Application.Admin.Users.DTOs;
using Application.Auth.DTOs;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Users.Commands.AdminResendInvite
{
    public class AdminResendInviteCommandHandler : IRequestHandler<AdminResendInviteCommand, InviteUserResponse>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;
        private readonly IUserInviteService _inviteService;

        public AdminResendInviteCommandHandler(
            IApplicationDbContext db,
            ICurrentUserService current,
            IUserInviteService inviteService)
        {
            _db = db;
            _current = current;
            _inviteService = inviteService;
        }

        public async Task<InviteUserResponse> Handle(AdminResendInviteCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();
            var adminId = _current.UserId.Value;

            var user = await _db.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, ct);

            if (user == null) throw new InvalidOperationException("User not found.");
            if (user.IsDeleted) throw new InvalidOperationException("User is deleted.");

            var email = string.IsNullOrWhiteSpace(request.Request.Email)
                ? user.Email
                : request.Request.Email.Trim();

            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("User email is required.");

            var invite = await _inviteService.IssueInviteAsync(user.Id, email!, adminId, ct);
            return new InviteUserResponse(invite.UserId, invite.Email, invite.RawToken, invite.ExpiresAt);
        }
    }
}
