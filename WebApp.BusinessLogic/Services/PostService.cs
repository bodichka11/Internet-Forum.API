using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApp.BusinessLogic.DTOs.Post;
using WebApp.BusinessLogic.Helpers;
using WebApp.BusinessLogic.Services.Interfaces;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.BusinessLogic.Services;
public class PostService : IPostService
{
    private readonly IPostRepository postRepository;
    private readonly ITagRepository tagRepository;
    private readonly IMapper mapper;
    private readonly ILogger<PostService> logger;
    private readonly IWebHostEnvironment environment;

    public PostService(
        IPostRepository postRepository,
        IMapper mapper,
        ILogger<PostService> logger,
        ITagRepository tagRepository,
        IWebHostEnvironment emvironment)
    {
        this.postRepository = postRepository;
        this.tagRepository = tagRepository;
        this.mapper = mapper;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.environment = emvironment ?? throw new ArgumentNullException(nameof(emvironment));
    }

    public async Task<PostDto> CreatePost(CreatePostDto createPostDto, List<IFormFile> images)
    {
        ValidateCreatePostParameters(createPostDto);

        Post post = this.mapper.Map<Post>(createPostDto);

        if (createPostDto.Tags != null)
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            post.Tags = new List<Tag>();
#pragma warning restore IDE0028 // Simplify collection initialization

#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
            foreach (var tagDto in createPostDto.Tags)
            {
                var existingTag = await this.tagRepository.GetTagByNameAsync(tagDto.Name);

                if (existingTag != null)
                {
                    post.Tags.Add(existingTag);
                }
                else
                {
                    var newTag = new Tag { Name = tagDto.Name };
                    await this.tagRepository.AddTagAsync(newTag);
                    post.Tags.Add(newTag);
                }
            }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
        }

        if (images != null && images.Count > 0)
        {
            post.Images = await this.SaveImages(images);
        }

        await this.postRepository.CreatePostAsync(post);
        await this.postRepository.SaveChangesAsync();

        post.Link = GeneratePostLink(post.Id, createPostDto.UserName, createPostDto.Title);

        await this.postRepository.UpdatePostAsync(post);
        await this.postRepository.SaveChangesAsync();

        PostServiceLogging.PostCreated(this.logger, post.Title);
        return this.mapper.Map<PostDto>(post);
    }

    public async Task DeletePost(long id)
    {
        var post = await this.postRepository.GetPostByIdAsync(id);

        if (post == null)
        {
            PostServiceLogging.PostActionFailed(this.logger, $"Post with ID {id}");
            throw new KeyNotFoundException("Post not found");
        }

        await this.postRepository.DeletePostAsync(id);
        await this.postRepository.SaveChangesAsync();

        PostServiceLogging.PostDeleted(this.logger, post.Title);
    }

    public async Task<PostDto> UpdatePost(long id, PostUpdateDto postUpdateDto, List<IFormFile> images)
    {
        ValidateUpdatePostParameters(postUpdateDto);

        var existingPost = await this.postRepository.GetPostByIdAsync(id);
        if (existingPost == null)
        {
            throw new KeyNotFoundException("Post not found");
        }

        _ = this.mapper.Map(postUpdateDto, existingPost);

        if (postUpdateDto.Tags != null)
        {
            existingPost.Tags.Clear();

#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
            foreach (var tagDto in postUpdateDto.Tags)
            {
                var existingTag = await this.tagRepository.GetTagByNameAsync(tagDto.Name);
                if (existingTag != null)
                {
                    existingPost.Tags.Add(existingTag);
                }
                else
                {
                    var newTag = new Tag { Name = tagDto.Name };
                    await this.tagRepository.AddTagAsync(newTag);
                    existingPost.Tags.Add(newTag);
                }
            }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
        }

        if (images != null && images.Count > 0)
        {
            existingPost.Images = await this.SaveImages(images);
        }

        await this.postRepository.UpdatePostAsync(existingPost);
        await this.postRepository.SaveChangesAsync();

        return this.mapper.Map<PostDto>(existingPost);
    }

    public async Task<(IEnumerable<PostDto> Posts, int TotalItems)> GetPostsAsync(int page, int pageSize)
    {
        var (posts, totalItems) = await this.postRepository.GetPostsAsync(page, pageSize);
        return (this.mapper.Map<IEnumerable<PostDto>>(posts), totalItems);
    }

    public async Task<IEnumerable<PostDto>> GetPostsByTopicAsync(long topicId, int pageNumber, int pageSize)
    {
        var posts = await this.postRepository.GetPostsByTopicAsync(topicId, pageNumber, pageSize);
        return this.mapper.Map<IEnumerable<PostDto>>(posts);
    }

    public async Task<PostDto> GetPostByIdAsync(long id)
    {
        var post = await this.postRepository.GetPostByIdAsync(id);
        return this.mapper.Map<PostDto>(post);
    }

    public async Task<IEnumerable<PostDto>> GetPostsByUserIdAsync(long userId, int pageNumber, int pageSize)
    {
        var posts = await this.postRepository.GetPostsByUserIdAsync(userId, pageNumber, pageSize);
        return this.mapper.Map<IEnumerable<PostDto>>(posts);
    }

    public async Task<IEnumerable<PostDto>> GetPopularPostsAsync(int count)
    {
        var popularPosts = await this.postRepository.GetPopularPostsAsync(count);
        return this.mapper.Map<IEnumerable<PostDto>>(popularPosts);
    }

    public async Task<IEnumerable<PostDto>> SearchPostsByTitleAsync(string title, int page, int pageSize)
    {
        var posts = await this.postRepository.SearchPostsByTitleAsync(title, page, pageSize);
        return this.mapper.Map<IEnumerable<PostDto>>(posts);
    }

    private static void ValidateCreatePostParameters(CreatePostDto createPostDto)
    {
        ArgumentNullException.ThrowIfNull(createPostDto);
    }

    public string GeneratePostLink(long postId, string userName, string title)
    {
        string baseUrl = "https://localhost:7070";
        string formattedTitle = title.Replace(" ", "-").Replace("?", "").Replace("!", "").Replace(".", "").Replace(",", "").ToLower();
        return $"{baseUrl}/posts/{postId}?user={userName}&title={formattedTitle}";
    }

    private static void ValidateUpdatePostParameters(PostUpdateDto postUpdateDto)
    {
        ArgumentNullException.ThrowIfNull(postUpdateDto);
    }

    private async Task<List<string>> SaveImages(List<IFormFile> images)
    {
        var imageUrls = new List<string>();
        foreach (var image in images)
        {
            if (image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var uploadsFolder = Path.Combine(this.environment.WebRootPath, "images");
                var filePath = Path.Combine(uploadsFolder, fileName);

                _ = Directory.CreateDirectory(uploadsFolder);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imageUrls.Add("/images/" + fileName);
            }
        }

        return imageUrls;
    }
}
