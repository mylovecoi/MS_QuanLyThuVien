using IdentityService.DataAccess.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Business.Helpers
{

    public static class JwtHelper
    {
        public static string GenerateToken(
            UserEntity user,
            IEnumerable<string> roles,
            IEnumerable<string> permissions,
            string jti,
            string secretKey,
            string issuer,
            string audience,
            int expirationMinutes = 480)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey.PadRight(32, '0'));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            // Thêm các role claims
            if (roles != null)
            {
                foreach (var role in roles.Distinct())
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            // Thêm các permission claims
            if (permissions != null)
            {
                foreach (var permission in permissions.Distinct())
                {
                    claims.Add(new Claim("permission", permission));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static (string? Jti, DateTime ExpiryTime)? GetJtiAndExpiryFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                if (!tokenHandler.CanReadToken(token)) return null;

                var jwtToken = tokenHandler.ReadJwtToken(token);
                var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti || c.Type == "jti");

                if (jtiClaim == null) return null;

                return (jtiClaim.Value, jwtToken.ValidTo);
            }
            catch
            {
                return null;
            }
        }
    }

    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password + "_secret_salt"));
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
    }
}
