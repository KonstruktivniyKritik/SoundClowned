using IdentityProvider.Data;

namespace IdentityProvider.Repositories
{
    /// <summary>
    /// Контракт для работы с данными пользователей.
    /// Изолирует логику доступа к данным от бизнес-логики.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Асинхронно находит пользователя по имени.
        /// </summary>
        /// <param name="username">Имя пользователя.</param>
        /// <returns>Объект ApplicationUser или null, если не найден.</returns>
        Task<ApplicationUser?> FindByUsernameAsync(string email);

        Task<ApplicationUser> AddUserAsync(ApplicationUser user);
    }
}
