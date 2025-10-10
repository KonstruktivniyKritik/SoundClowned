using System.Security.Claims;

namespace IdentityProvider.Services
{
    /// <summary>
    /// Определяет контракт для работы с паролями.
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Хеширует пароль
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string HashPassword(string password);
        /// <summary>
        /// Сравнивает пароли
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        bool VerifyPassword(string password, string hash);
    }
}
