namespace IdentityProvider.DTOs;

// Модель для входящих данных авторизации
public record LoginRequest(string Email, string Password);