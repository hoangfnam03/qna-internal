using Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity.Jwt
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _opt;

        public JwtTokenGenerator(IOptions<JwtOptions> opt)
        {
            _opt = opt.Value;
        }

        public (string token, DateTime expiresAtUtc) Generate(Guid userId, string? email, IEnumerable<string> roles)
        {
            var now = DateTime.UtcNow;
            var exp = now.AddMinutes(_opt.AccessTokenMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, userId.ToString())
            };

            if (!string.IsNullOrWhiteSpace(email))
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));

            foreach (var role in roles ?? Enumerable.Empty<string>())
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: now,
                expires: exp,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(jwt), exp);
        }
    }
}
