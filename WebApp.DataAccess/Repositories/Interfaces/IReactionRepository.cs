using WebApp.DataAccess.Entities;

namespace WebApp.DataAccess.Repositories.Interfaces;
public interface IReactionRepository
{
    Task<Reaction?> GetReactionByPostOrCommentAndUserAsync(long? postId, long? commentId, long userId);

    Task<Reaction?> GetReactionByIdAsync(long id);

    Task AddReactionAsync(Reaction reaction);

    Task UpdateReactionAsync(Reaction reaction);

    Task DeleteReactionAsync(long reactionId);

    Task<IEnumerable<Reaction>> GetReactionsByPostAsync(long postId, int pageNumber, int pageSize);

    Task<IEnumerable<Reaction>> GetReactionsByCommentAsync(long commentId, int pageNumber, int pageSize);

    Task SaveChangesAsync();
}
