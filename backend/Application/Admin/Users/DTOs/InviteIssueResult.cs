namespace Application.Admin.Users.DTOs
{
    public record InviteIssueResult(
        Guid UserId,
        string Email,
        string RawToken,
        DateTime ExpiresAt
    );
}
