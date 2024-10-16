using Microsoft.EntityFrameworkCore;
using WebApp.DataAccess.DataContext;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.DataAccess.Repositories;
public class TagRepository : ITagRepository
{
    private readonly ForumDbContext context;

    public TagRepository(ForumDbContext context)
    {
        this.context = context;
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        return await this.context.Set<Tag>()
            .FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task AddTagToPostAsync(long postId, Tag tag)
    {
        var post = await this.context.Set<Post>()
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post != null && !post.Tags.Any(t => t.Name == tag.Name))
        {
            post.Tags.Add(tag);
            _ = await this.context.SaveChangesAsync();
        }
    }

    public async Task<Tag?> GetTagByIdAsync(long id)
    {
        return await this.context.Tags.FindAsync(id);
    }

    public async Task AddTagAsync(Tag tag)
    {
        _ = await this.context.Tags.AddAsync(tag);
    }
}
