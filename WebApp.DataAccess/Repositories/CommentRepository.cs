using Microsoft.EntityFrameworkCore;
using WebApp.DataAccess.DataContext;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.DataAccess.Repositories;
public class CommentRepository : ICommentRepository
{
    private readonly ForumDbContext context;

    public CommentRepository(ForumDbContext context)
    {
        this.context = context;
    }

    public async Task AddCommentAsync(Comment comment)
    {
        bool userExists = await this.context.Users.AnyAsync(u => u.Id == comment.UserId);
        if (!userExists)
        {
            throw new InvalidOperationException("User does not exist.");
        }

        _ = await this.context.AddAsync(comment);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(long postId, int pageNumber, int pageSize)
    {
        return await this.context.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.Reactions)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Comment?> GetCommentByIdAsync(long id)
    {
        return await this.context.Comments
            .Include(c => c.Reactions)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateCommentAsync(Comment comment)
    {
        _ = this.context.Update(comment);
        await this.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(long id)
    {
        var comment = await this.GetCommentByIdAsync(id);
        if (comment != null)
        {
            _ = this.context.Comments.Remove(comment);
        }
    }

    public async Task SaveChangesAsync()
    {
        _ = await this.context.SaveChangesAsync();
    }

    public async Task<Comment?> GetCommentById(long id)
    {
        return await this.context.Comments
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
