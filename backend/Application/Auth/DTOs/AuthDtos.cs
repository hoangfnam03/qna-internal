namespace Application.Auth.DTOs
{
    public record InviteUserRequest(string Email, string DisplayName, string? UserName, List<string>? Roles);
    public record InviteUserResponse(Guid UserId, string Email, string InviteToken, DateTime ExpiresAtUtc);

    public record SetPasswordFromInviteRequest(string InviteToken, string NewPassword);

    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string AccessToken, DateTime AccessTokenExpiresAtUtc, string RefreshToken);

    public record RefreshRequest(string RefreshToken);

    public record LogoutRequest(string RefreshToken);

    public record ChangePasswordRequest(string OldPassword, string NewPassword);

    public record ForgotPasswordRequest(string Email);
    public record ForgotPasswordResponse(string Message, string? ResetToken, DateTime? ExpiresAtUtc);

    public record ResetPasswordRequest(string ResetToken, string NewPassword);
}
