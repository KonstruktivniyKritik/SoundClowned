using System;

namespace IdentityProvider.Events
{
    /// <summary>
    /// Контракт данных для события регистрации нового пользователя.
    /// Используется для отправки в Kafka.
    /// </summary>
    /// <param name="UserId">Уникальный идентификатор пользователя (Guid).</param>
    /// <param name="Username">Имя пользователя.</param>
    /// <param name="Email">Email пользователя.</param>
    /// <param name="Timestamp">Время создания события (для аудита).</param>
    public record UserRegisteredEvent(Guid UserId, string Username, string Email, DateTime Timestamp);
}