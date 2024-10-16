using WebApp.DataAccess.Entities;

namespace WebApp.DataAccess.Repositories.Interfaces;
public interface ITopicRepository
{
    Task<IEnumerable<Topic>> GetAllTopicAsync(int pageNumber, int pageSize);

    Task<Topic?> GetTopicByIdAsync(long id);

    Task CreateTopicAsync(Topic topic);

    Task DeleteTopicAsync(long id);

    Task UpdateTopicAsync(Topic topic);

    Task SaveChangesAsync();
}
