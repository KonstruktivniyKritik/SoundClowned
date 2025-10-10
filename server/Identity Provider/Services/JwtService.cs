using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityProvider.Configuration;
using IdentityProvider.Services;
using Microsoft.IdentityModel.Tokens;

namespace IdentityProvider.Services
{
    /// <summary>
    /// Сервис для генерации JWT-токенов.
    /// </summary>
    public class JwtService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            // 1. Создание ключа подписи
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2. Определение времени жизни токена
            var expires = DateTime.Now.AddDays(7); // Токен действителен 7 дней

            // 3. Создание описателя токена (Token Descriptor)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Expires = expires,
                SigningCredentials = credentials
            };

            // 4. Генерация токена
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 5. Сериализация в строку
            return tokenHandler.WriteToken(token);
        }
    }
}
