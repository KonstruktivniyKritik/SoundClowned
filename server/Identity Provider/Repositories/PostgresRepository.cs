using IdentityProvider.Data;
using IdentityProvider.Repositories;
using Microsoft.EntityFrameworkCore;

public class PostgresRepository : IUserRepository
{
    IdentityDbContext _context;

    public PostgresRepository(IdentityDbContext dbContext)
    {
        _context = dbContext;
    }
    public async Task<ApplicationUser?> FindByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
    }

    public async Task<ApplicationUser> AddUserAsync(ApplicationUser user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}