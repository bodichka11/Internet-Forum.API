using Microsoft.EntityFrameworkCore;
using WebApp.DataAccess.DataContext;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.DataAccess.Repositories;
public class TopicRepository : ITopicRepository
{
    private readonly ForumDbContext context;

    public TopicRepository(ForumDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Topic>> GetAllTopicAsync(int pageNumber, int pageSize)
    {
        var topics = await this.context.Topics
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return topics;
    }

    public async Task<Topic?> GetTopicByIdAsync(long id)
    {
        return await this.context.Topics
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task CreateTopicAsync(Topic topic)
    {
        _ = await this.context.Topics.AddAsync(topic);
    }

    public async Task UpdateTopicAsync(Topic topic)
    {
        _ = this.context.Topics.Update(topic);
        _ = await this.context.SaveChangesAsync();
    }

    public async Task DeleteTopicAsync(long id)
    {
        var post = await this.GetTopicByIdAsync(id);
        if (post != null)
        {
            _ = this.context.Topics.Remove(post);
        }
    }

    public async Task SaveChangesAsync()
    {
        _ = await this.context.SaveChangesAsync();
    }
}
