using Microsoft.EntityFrameworkCore;
using WebApp.DataAccess.DataContext;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.DataAccess.Repositories;
public class PostRepository : IPostRepository
{
    private readonly ForumDbContext context;

    public PostRepository(ForumDbContext context)
    {
        this.context = context;
    }

    public async Task<(IEnumerable<Post> Posts, int TotalItems)> GetPostsAsync(int page, int pageSize)
    {
        var posts = await this.context.Posts
            .Include(p => p.Topic)
            .Include(p => p.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Reactions)
            .Include(p => p.Reactions)
            .Include(p => p.Tags)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalItems = await this.context.Posts.CountAsync();
        return (posts, totalItems);
    }

    public async Task<Post?> GetPostByIdAsync(long id)
    {
        return await this.context.Posts
            .Include(p => p.Topic)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Post?> GetPostByIdOrLinkAsync(string idOrLink)
    {
        if (long.TryParse(idOrLink, out long id))
        {
            return await this.context.Posts
                .Include(p => p.Topic)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        else
        {
            return await this.context.Posts
                .Include(p => p.Topic)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Link == idOrLink);
        }
    }

    public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(long userId, int pageNumber, int pageSize)
    {
        return await this.context.Posts
            .Where(p => p.UserId == userId)
            .Include(p => p.Topic)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .Include(p => p.Tags)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task CreatePostAsync(Post post)
    {
        _ = await this.context.Posts.AddAsync(post);
    }

    public async Task UpdatePostAsync(Post post)
    {
        _ = this.context.Posts.Update(post);
        await this.SaveChangesAsync();
    }

    public async Task DeletePostAsync(long id)
    {
        var post = await this.GetPostByIdAsync(id);
        if (post != null)
        {
            _ = this.context.Posts.Remove(post);
        }
    }

    public async Task<IEnumerable<Post>> GetPostsByTopicAsync(long topicId, int pageNumber, int pageSize)
    {
        return await this.context.Posts
            .Where(p => p.TopicId == topicId)
            .Include(p => p.Topic)
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .Include(p => p.Tags)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        _ = await this.context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Post>> GetPopularPostsAsync(int count)
    {
        return await this.context.Posts
                            .Include(p => p.Topic)
                            .Include(p => p.User)
                            .Include(p => p.Comments)
                            .Include(p => p.Reactions)
                            .Include(p => p.Tags)
                            .OrderByDescending(p => p.Reactions.Count)
                            .Take(count)
                            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> SearchPostsByTitleAsync(string title, int page, int pageSize)
    {
        return await this.context.Posts
                            .Where(p => EF.Functions.Like(p.Title, $"%{title}%"))
                            .Include(p => p.Topic)
                            .Include(p => p.User)
                            .Include(p => p.Comments)
                            .Include(p => p.Reactions)
                            .Include(p => p.Tags)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
    }
}
