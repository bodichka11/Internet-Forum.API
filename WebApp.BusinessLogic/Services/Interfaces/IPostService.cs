using Microsoft.AspNetCore.Http;
using WebApp.BusinessLogic.DTOs.Post;

namespace WebApp.BusinessLogic.Services.Interfaces;
public interface IPostService
{
#pragma warning disable CA1002 // Do not expose generic lists
    Task<PostDto> CreatePost(CreatePostDto createPostDto, List<IFormFile> images);

    Task DeletePost(long id);

    Task<PostDto> UpdatePost(long id, PostUpdateDto postUpdateDto, List<IFormFile> images);
#pragma warning restore CA1002 // Do not expose generic lists

    Task<(IEnumerable<PostDto> Posts, int TotalItems)> GetPostsAsync(int page, int pageSize);

    Task<IEnumerable<PostDto>> GetPostsByTopicAsync(long topicId, int pageNumber, int pageSize);

    Task<PostDto> GetPostByIdAsync(long id);

    Task<IEnumerable<PostDto>> GetPostsByUserIdAsync(long userId, int pageNumber, int pageSize);

    Task<IEnumerable<PostDto>> GetPopularPostsAsync(int count);

    Task<IEnumerable<PostDto>> SearchPostsByTitleAsync(string title, int page, int pageSize);

    string GeneratePostLink(long postId, string userName, string title);
}
