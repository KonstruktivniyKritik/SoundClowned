using IdentityProvider.Services;
using BCrypt.Net;

namespace IdentityProvider.Services
{
    /// <summary>
    /// Сервис для хэширования и проверки паролей с использованием алгоритма BCrypt.
    /// </summary>
    public class BCryptPasswordService : IPasswordService
    {
        // Рекомендуемая сложность хэширования
        private const int WorkFactor = 12;

        /// <summary>
        /// Создает безопасный хэш для пароля.
        /// </summary>
        /// <param name="password">Чистый пароль.</param>
        /// <returns>Строка с хэшем пароля.</returns>
        public string HashPassword(string password)
        {
            // BCrypt.Net автоматически генерирует соль и добавляет ее к хэшу.
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        /// <summary>
        /// Проверяет чистый пароль на соответствие сохраненному хэшу.
        /// </summary>
        /// <param name="password">Чистый пароль, введенный пользователем.</param>
        /// <param name="hash">Хэш, сохраненный в базе данных.</param>
        /// <returns>True, если пароль совпадает с хэшем, иначе False.</returns>
        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}