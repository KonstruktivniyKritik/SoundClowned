namespace IdentityProvider.DTOs;

// Модель для ответа с JWT-токеном
public record LoginResponse(string message, string token);