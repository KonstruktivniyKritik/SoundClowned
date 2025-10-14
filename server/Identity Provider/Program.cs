// Убедитесь, что эта директива есть
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IdentityProvider.Configuration;
using IdentityProvider.DTOs;
using IdentityProvider.Endpoints;
using IdentityProvider.Services;
using IdentityProvider.Repositories;
using IdentityProvider.Data;
using Microsoft.EntityFrameworkCore;
using IdentityProvider.Extensions;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ДОБАВЛЕНИЕ СЕРВИСОВ (builder.Services) ---

// 1.1. Настройка JWT (здесь оставим без изменений, но это важно)
var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
if (jwtSettings == null)
{
    throw new InvalidOperationException("JWT settings are missing in configuration.");
}
builder.Services.AddSingleton(jwtSettings);

//1.2. Подключение Postgres
var connectionString = builder.Configuration.GetConnectionString("PostgresDb");

if (string.IsNullOrEmpty(connectionString))
{
    // Это должно произойти, если переменная окружения не задана в docker-compose.yml
    throw new InvalidOperationException("Connection string 'PostgresDb' not found.");
}

builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// 1.3. ДОБАВЛЕНИЕ СЕРВИСА АУТЕНТИФИКАЦИИ JWT
// Identity Provider сам не будет использовать эту аутентификацию (т.к. он ее выдает), 
// но эта настройка необходима для корректного построения JWT-токена.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });
builder.Services.AddAuthorization();

// 1.4. Добавление CORS-сервиса
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        // Gateway: http://api-gateway (внутреннее имя Docker-сервиса)
        policy.WithOrigins("http://api-gateway") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Разрешаем куки и заголовки авторизации
    });
});

// Добавляем Swagger (для тестирования)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddTransient<ITokenService, JwtService>();
builder.Services.AddTransient<IPasswordService, BCryptPasswordService>();
builder.Services.AddTransient<IUserRepository, PostgresRepository>();
builder.Services.AddTransient<IEventProducer, KafkaProducerService>();

var app = builder.Build();

app.MigrateDatabase();

// --- 2. КОНФИГУРАЦИЯ MIDDLEWARE (app.Use...) ---

// 2.0. Добавление маршрутизации для надежности
app.UseRouting();

// 2.1. Swagger UI/Docs (Переносим сюда для отладки, чтобы работало всегда)
app.UseSwagger();
app.UseSwaggerUI();

// 2.2. Использование CORS-middleware
app.UseCors("CorsPolicy");

// 2.3. Использование сервиса аутентификации
// Обязательно идет до UseEndpoints/MapControllers
app.UseAuthentication();
app.UseAuthorization();

// 2.4. Свои endpoints
app.MapAuthEndpoints();

app.Run();