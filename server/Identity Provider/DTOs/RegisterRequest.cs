using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.DTOs;

/// <summary>
/// Модель для входящих данных запроса регистрации нового пользователя.
/// </summary>
public record RegisterRequest(
    [Required] string Username, 
    [Required] [EmailAddress] string Email, // Добавляем email для большей реалистичности
    [Required] string Password
);
