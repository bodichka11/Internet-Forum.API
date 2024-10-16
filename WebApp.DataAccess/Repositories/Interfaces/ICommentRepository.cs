using WebApp.DataAccess.Entities;

namespace WebApp.DataAccess.Repositories.Interfaces;
public interface ICommentRepository
{
    Task AddCommentAsync(Comment comment);

    Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(long postId, int pageNumber, int pageSize);

    Task<Comment?> GetCommentByIdAsync(long id);

    Task UpdateCommentAsync(Comment comment);

    Task DeleteCommentAsync(long id);

    Task<Comment?> GetCommentById(long id);

    Task SaveChangesAsync();
}
