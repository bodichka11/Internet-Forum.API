using WebApp.DataAccess.Entities;

namespace WebApp.DataAccess.Repositories.Interfaces;
public interface ITagRepository
{
    Task<Tag?> GetTagByNameAsync(string name);

    Task AddTagToPostAsync(long postId, Tag tag);

    Task<Tag?> GetTagByIdAsync(long id);

    Task AddTagAsync(Tag tag);
}
