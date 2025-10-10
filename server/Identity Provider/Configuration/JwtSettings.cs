namespace IdentityProvider.Configuration;

// Класс для маппинга настроек JWT из конфигурационных источников (docker-compose.yml или appsettings.json)
public class JwtSettings
{
    // Секретный ключ для подписи токена (ОЧЕНЬ ВАЖНО: Должен быть длинным и сложным)
    public string Secret { get; set; } = string.Empty;
    
    // Издатель (кем выпущен токен), например, "SoundclownedIdentity"
    public string Issuer { get; set; } = string.Empty;
    
    // Аудитория (для кого предназначен токен), например, "SoundclownedApi"
    public string Audience { get; set; } = string.Empty;
}
