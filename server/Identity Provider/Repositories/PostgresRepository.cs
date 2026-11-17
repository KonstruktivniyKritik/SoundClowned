using IdentityProvider.Data;
using IdentityProvider.Repositories;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

public class PostgresRepository : IUserRepository
{
    IdentityDbContext _context;
    ILogger<PostgresRepository> _logger;

    public PostgresRepository(IdentityDbContext dbContext, ILogger<PostgresRepository> logger) 
    {
        _context = dbContext;
        _logger = logger;
    }
    public async Task<ApplicationUser?> FindByUsernameAsync(string email)
    {
        _logger.LogInformation($"Ищу {email}");
        var result =  await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
        if (result == null)
        {
            _logger.LogInformation($"Нету такого пользователя");
        }
        return result;
    }

    public async Task<ApplicationUser> AddUserAsync(ApplicationUser user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}