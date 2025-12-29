using Domain.Identity.Enums;

namespace Application.Admin.Users.DTOs
{
    public record AdminUserListItemDto(
        Guid Id,
        string UserName,
        string DisplayName,
        string? Email,
        UserStatus Status,
        DateTime? SuspendedUntil,
        string? SuspensionReason,
        DateTime CreatedAt,
        List<string> Roles
    );

    public record AdminUserDetailDto(
        Guid Id,
        string UserName,
        string DisplayName,
        string? Email,
        bool EmailConfirmed,
        UserStatus Status,
        DateTime? SuspendedUntil,
        string? SuspensionReason,
        string? Bio,
        string? AvatarUrl,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        DateTime? DeletedAt,
        bool IsDeleted,
        List<string> Roles
    );

    public record AdminUserSuspendRequest(
        DateTime? SuspendedUntil,
        string? Reason
    );

    public record AdminSetUserRolesRequest(
        List<string> Roles
    );

    public record AdminResendInviteRequest(
        string? Email // nếu null thì dùng user.Email
    );

    public record AdminInviteResultDto(
        Guid UserId,
        DateTime ExpiresAt,
        string InviteLink
    );
}
