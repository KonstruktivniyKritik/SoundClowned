using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

// Загрузка конфигурации Ocelot из файла ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot();

// =========================================================================
// 🚀 НАСТРОЙКА JWT АУТЕНТИФИКАЦИИ ДЛЯ GATEWAY
// =========================================================================

// Получаем настройки JWT из переменных окружения (docker-compose)
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

// Проверяем, что секретный ключ установлен
if (string.IsNullOrEmpty(jwtSecret))
{
    // Это критическая ошибка, если ключ не установлен
    throw new InvalidOperationException("JwtSettings:Secret не настроен. Проверьте docker-compose.yml.");
}

// Регистрируем схему аутентификации
builder.Services.AddAuthentication(options =>
{
    // Указываем, что по умолчанию используем схему Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // 1. Проверяем подписывающий ключ (самое важное)
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        
        // 2. Проверяем издателя токена
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,

        // 3. Проверяем аудиторию
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        
        // 4. Проверяем время жизни токена
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Точное время жизни токена
    };
});

// Настройка CORS (ОЧЕНЬ ВАЖНО для фронтенда React)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        // Разрешаем запросы с любого источника (для localhost:3000)
        policy.AllowAnyOrigin() 
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 🚀 ВАЖНО: АУТЕНТИФИКАЦИЯ ДОЛЖНА БЫТЬ ПЕРЕД OCELOT
app.UseAuthentication();

// Использование CORS
app.UseCors("CorsPolicy");

// Использование Ocelot Middleware
await app.UseOcelot();

app.Run();
