// Infrastructure/Identity/PasswordHashing/PasswordHasherService.cs
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.PasswordHashing
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string Hash(string password)
            => _hasher.HashPassword(new object(), password);

        public bool Verify(string password, string passwordHash)
            => _hasher.VerifyHashedPassword(new object(), passwordHash, password)
               == PasswordVerificationResult.Success;
    }
}
