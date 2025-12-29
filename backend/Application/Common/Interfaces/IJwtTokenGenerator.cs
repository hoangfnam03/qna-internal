namespace Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        (string token, DateTime expiresAtUtc) Generate(Guid userId, string? email, IEnumerable<string> roles);
    }
}
