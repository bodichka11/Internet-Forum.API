using Microsoft.EntityFrameworkCore;
using WebApp.DataAccess.DataContext;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.DataAccess.Repositories;
public class UserRepository : IUserRepository
{
    private readonly ForumDbContext context;

    public UserRepository(ForumDbContext context)
    {
        this.context = context;
    }

    public async Task<User?> GetUserByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await this.context.Users
            .Include(u => u.Posts)
                .ThenInclude(p => p.Comments)
            .Include(u => u.Comments)
            .Include(u => u.Reactions)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await this.context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        _ = await this.context.Users.AddAsync(user, cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        _ = this.context.Users.Update(user);
        _ = await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        _ = await this.context.SaveChangesAsync(cancellationToken);
    }
}
