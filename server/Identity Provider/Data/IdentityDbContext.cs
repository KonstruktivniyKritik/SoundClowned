using Microsoft.EntityFrameworkCore;
using IdentityProvider.Data;
using System;

namespace IdentityProvider.Data
{
    /// <summary>
    /// Контекст базы данных для Identity Provider, наследуется от DbContext.
    /// Отвечает за управление сеансами с базой данных и отображение моделей (ApplicationUser) в таблицы.
    /// </summary>
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        // Коллекция пользователей в базе данных (соответствует таблице "Users")
        public DbSet<ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Конфигурация модели ApplicationUser ---
            
            // Ограничение: Username должен быть уникальным
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // 2. Ограничение: Email должен быть уникальным
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
