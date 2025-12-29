using Application.Admin.Users.DTOs;

namespace Application.Common.Interfaces
{
    public interface IUserInviteService
    {
        Task<InviteIssueResult> IssueInviteAsync(Guid userId, string email, Guid createdByUserId, CancellationToken ct);
    }
}
