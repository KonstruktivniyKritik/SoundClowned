using IdentityProvider.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.Extensions
{
    /// <summary>
    /// Статический класс для методов расширения, связанных с инициализацией базы данных.
    /// Этот класс помогает отделить логику инициализации от Program.cs.
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Инициализирует базу данных, вызывая EnsureCreated.
        /// Этот подход считается более чистым, так как отделяет логику инициализации
        /// от основного кода запуска приложения.
        /// </summary>
        /// <param name="app">Экземпляр WebApplication.</param>
        public static void MigrateDatabase(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<IdentityDbContext>();
                
                try
                {
                    // ВАЖНО: Применяет миграции или создает БД, если она не существует
                    // Для Production рекомендуется использовать Migrate()
                    context.Database.EnsureCreated(); 
                    app.Logger.LogInformation("PostgreSQL database ensured to be created and seeded.");
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "An error occurred while initializing the database.");
                }
            }
        }
    }
}
