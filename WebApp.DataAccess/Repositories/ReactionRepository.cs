using Microsoft.EntityFrameworkCore;
using WebApp.DataAccess.DataContext;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.DataAccess.Repositories;
public class ReactionRepository : IReactionRepository
{
    private readonly ForumDbContext context;

    public ReactionRepository(ForumDbContext context)
    {
        this.context = context;
    }

    public async Task<Reaction?> GetReactionByPostOrCommentAndUserAsync(long? postId, long? commentId, long userId)
    {
        return await this.context.Reactions
            .FirstOrDefaultAsync(r =>
                ((r.PostId == postId && postId != null) ||
            (r.CommentId == commentId && commentId != null))
                && r.UserId == userId);
    }

    public async Task<Reaction?> GetReactionByIdAsync(long id)
    {
        return await this.context.Reactions
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddReactionAsync(Reaction reaction)
    {
        _ = await this.context.Reactions.AddAsync(reaction);
    }

    public async Task UpdateReactionAsync(Reaction reaction)
    {
        _ = this.context.Reactions.Update(reaction);
        await this.SaveChangesAsync();
    }

    public async Task DeleteReactionAsync(long reactionId)
    {
        var reaction = await this.GetReactionByIdAsync(reactionId);
        if (reaction != null)
        {
            _ = this.context.Reactions.Remove(reaction);
        }
    }

    public async Task<IEnumerable<Reaction>> GetReactionsByPostAsync(long postId, int pageNumber, int pageSize)
    {
        return await this.context.Reactions
            .Where(r => r.PostId == postId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reaction>> GetReactionsByCommentAsync(long commentId, int pageNumber, int pageSize)
    {
        return await this.context.Reactions
            .Where(r => r.CommentId == commentId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        _ = await this.context.SaveChangesAsync();
    }
}
