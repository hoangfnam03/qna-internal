using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Utils
{
    public static class TokenUtils
    {
        public static string NewRawToken(int bytes = 32)
        {
            var buf = RandomNumberGenerator.GetBytes(bytes);
            // base64url
            return Convert.ToBase64String(buf).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public static string Sha256Hex(string raw)
        {
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant(); // 64 chars
        }
    }
}
