
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace DalilAcademy.Infra.Repositorie;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email)
        => context.Users
                  .AsNoTracking()
                  .FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
}