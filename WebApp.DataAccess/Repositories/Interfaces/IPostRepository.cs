using WebApp.DataAccess.Entities;

namespace WebApp.DataAccess.Repositories.Interfaces;
public interface IPostRepository
{
    Task<(IEnumerable<Post> Posts, int TotalItems)> GetPostsAsync(int page, int pageSize);

    Task<Post?> GetPostByIdAsync(long id);

    Task<Post?> GetPostByIdOrLinkAsync(string idOrLink);

    Task<IEnumerable<Post>> GetPostsByUserIdAsync(long userId, int pageNumber, int pageSize);

    Task CreatePostAsync(Post post);

    Task UpdatePostAsync(Post post);

    Task DeletePostAsync(long id);

    Task<IEnumerable<Post>> GetPostsByTopicAsync(long topicId, int pageNumber, int pageSize);

    Task SaveChangesAsync();

    Task<IEnumerable<Post>> GetPopularPostsAsync(int count);

    Task<IEnumerable<Post>> SearchPostsByTitleAsync(string title, int page, int pageSize);
}
