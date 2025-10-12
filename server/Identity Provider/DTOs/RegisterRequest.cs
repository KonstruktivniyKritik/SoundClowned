using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.DTOs;

/// <summary>
/// Модель для входящих данных запроса регистрации нового пользователя.
/// </summary>
public record RegisterRequest(
    [Required] [MinLength(3)] [MaxLength(50)] string Username, 
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(8)] [MaxLength(100)] string Password
);
