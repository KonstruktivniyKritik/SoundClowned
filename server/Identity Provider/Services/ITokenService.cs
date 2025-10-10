using System.Security.Claims;

namespace IdentityProvider.Services
{
    /// <summary>
    /// Определяет контракт для сервиса генерации и подписи JWT-токенов.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Генерирует подписанный JWT-токен на основе заданного списка Claims.
        /// </summary>
        /// <param name="claims">Список утверждений (Claims) для включения в токен.</param>
        /// <returns>Строка, содержащая JWT-токен.</returns>
        string GenerateToken(IEnumerable<Claim> claims);
    }
}
