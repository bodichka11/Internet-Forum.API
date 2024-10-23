using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.BusinessLogic.DTOs.Post;
using WebApp.BusinessLogic.Services.Interfaces;
using WebApp.WebApi.Interfaces;

namespace WebApp.WebApi.Controllers;

/// <summary>
/// Controller for handling post-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService postService;
    private readonly IPostGenerator postGenerator;
    private readonly IMapper mapper;
    private readonly ILogger<PostController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostController"/> class.
    /// </summary>
    /// <param name="postService">The post service.</param>
    /// <param name="mapper">The object mapper.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="postGenerator">The post generator service.</param>
    public PostController(IPostService postService, IMapper mapper, ILogger<PostController> logger, IPostGenerator postGenerator)
    {
        this.postService = postService;
        this.mapper = mapper;
        this.logger = logger;
        this.postGenerator = postGenerator;
    }

    /// <summary>
    /// Fetches all posts with pagination.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A list of posts.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        this.logger.LogInformation("Fetching all posts with page: {Page}, pageSize: {PageSize}", page, pageSize);

        var (posts, totalItems) = await this.postService.GetPostsAsync(page, pageSize);
        return this.Ok(new { posts, totalItems });
    }

    /// <summary>
    /// Fetches all posts for a specific topic with pagination.
    /// </summary>
    /// <param name="topicId">The topic ID.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A list of posts.</returns>
    [HttpGet("topic/{topicId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPostsByTopic(long topicId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return this.BadRequest("Invalid pagination parameters");
        }

        this.logger.LogInformation("Getting posts by topic id {TopicId} with pagination (Page: {PageNumber}, Size: {PageSize})", topicId, pageNumber, pageSize);

        this.logger.LogInformation("Fetching posts by topicId: {TopicId}", topicId);

        var posts = await this.postService.GetPostsByTopicAsync(topicId, pageNumber, pageSize);
        return this.Ok(posts);
    }

    /// <summary>
    /// Fetches a post by its ID.
    /// </summary>
    /// <param name="id">The post ID.</param>
    /// <returns>The post if found.</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostByIdAsync(long id)
    {
        this.logger.LogInformation("Fetching post by id: {PostId}", id);

        var post = await this.postService.GetPostByIdAsync(id);
        if (post == null)
        {
            this.logger.LogWarning("Post not found with id: {PostId}", id);
            return this.NotFound();
        }

        return this.Ok(post);
    }

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <param name="createPostWithoutUserIdDto">The DTO for creating the post without the user ID.</param>
    /// <param name="images">The list of images associated with the post.</param>
    /// <returns>The created post.</returns>
    [HttpPost]
    [Authorize(Roles = "User, Admin")]
#pragma warning disable CA1002 // Do not expose generic lists
    public async Task<ActionResult<PostDto>> CreatePost([FromForm] CreatePostWithoutUserIdDto createPostWithoutUserIdDto, List<IFormFile> images)
#pragma warning restore CA1002 // Do not expose generic lists
    {
        if (createPostWithoutUserIdDto == null)
        {
            this.logger.LogWarning("CreatePost called with null DTO");
            return this.BadRequest("Post data is required.");
        }

        var userIdentifierClaim = this.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier);

        if (userIdentifierClaim == null)
        {
            this.logger.LogWarning("CreatePost called without user identifier");
            return this.Unauthorized();
        }

        try
        {
            var createPostDto = this.mapper.Map<CreatePostDto>(createPostWithoutUserIdDto);
            createPostDto.UserId = long.Parse(userIdentifierClaim.Value, CultureInfo.InvariantCulture);
            createPostDto.UserName = this.User.Identity.Name ?? "unknown";

            this.logger.LogInformation("Creating post for user {UserId}", createPostDto.UserId);

            var createdPost = await this.postService.CreatePost(createPostDto, images);

            if (createdPost == null || createdPost.Id <= 0)
            {
                this.logger.LogError("Invalid created post for user {UserId}", createPostDto.UserId);
                return this.BadRequest("Invalid created post.");
            }

            this.logger.LogInformation("Post created with id {PostId}", createdPost.Id);
            return this.Ok(createdPost);
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Invalid arguments provided while creating post.");
            return this.BadRequest("Invalid arguments provided.");
        }
    }

    /// <summary>
    /// Updates an existing post.
    /// </summary>
    /// <param name="id">The post ID.</param>
    /// <param name="postUpdateDto">The DTO with the post updates.</param>
    /// <param name="images">The list of images associated with the post.</param>
    /// <returns>The updated post.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "User, Admin")]
#pragma warning disable CA1002 // Do not expose generic lists
    public async Task<IActionResult> UpdatePost([FromRoute] long id, [FromForm] PostUpdateDto postUpdateDto, List<IFormFile> images)
#pragma warning restore CA1002 // Do not expose generic lists
    {
        this.logger.LogInformation("Updating post with id {PostId}", id);

        try
        {
            var updatedPost = await this.postService.UpdatePost(id, postUpdateDto, images);
            return this.Ok(updatedPost);
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogWarning(ex, "Post not found with id {PostId}", id);
            return this.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Invalid arguments provided while updating post with id {PostId}", id);
            return this.BadRequest("Invalid arguments provided.");
        }
    }

    /// <summary>
    /// Deletes a post by its ID.
    /// </summary>
    /// <param name="id">The post ID.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> DeletePost(long id)
    {
        this.logger.LogInformation("Deleting post with id {PostId}", id);

        try
        {
            await this.postService.DeletePost(id);
            this.logger.LogInformation("Post deleted with id {PostId}", id);
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogWarning(ex, "Post not found with id {PostId}", id);
            return this.NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Fetches the currently authenticated user's posts with pagination.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A list of the user's posts.</returns>
    [HttpGet("my-posts")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> GetMyPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return this.BadRequest("Invalid pagination parameters");
        }

        this.logger.LogInformation("Getting users posts with pagination (Page: {PageNumber}, Size: {PageSize})", pageNumber, pageSize);

        var userIdentifierClaim = this.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier);

        if (userIdentifierClaim == null)
        {
            this.logger.LogWarning("GetMyPosts called without user identifier");
            return this.Unauthorized();
        }

        var userId = long.Parse(userIdentifierClaim.Value, CultureInfo.InvariantCulture);
        var posts = await this.postService.GetPostsByUserIdAsync(userId, pageNumber, pageSize);

        return this.Ok(posts);
    }

    /// <summary>
    /// Fetches the most popular posts.
    /// </summary>
    /// <param name="count">The number of popular posts to fetch.</param>
    /// <returns>A list of popular posts.</returns>
    [HttpGet("popular")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPopularPosts([FromQuery] int count = 10)
    {
        this.logger.LogInformation("Fetching popular posts with count: {Count}", count);

        var posts = await this.postService.GetPopularPostsAsync(count);
        return this.Ok(posts);
    }

    /// <summary>
    /// Searches for posts by title with pagination.
    /// </summary>
    /// <param name="title">The title to search for.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A list of posts matching the title.</returns>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchPostsByTitle([FromQuery] string title, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        this.logger.LogInformation("Searching posts by title: {Title} with page: {Page}, pageSize: {PageSize}", title, page, pageSize);

        var posts = await this.postService.SearchPostsByTitleAsync(title, page, pageSize);
        return this.Ok(posts);
    }

    /// <summary>
    /// Generates a new post based on the provided title.
    /// </summary>
    /// <param name="createPostRequest">The DTO containing the title for the new post.</param>
    /// <returns>The generated post.</returns>
    [HttpPost("generate")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> GeneratePost([FromBody] CreatePostOnlyTitleRequestDto createPostRequest)
    {
        ValidatePostForGeneratingParameters(createPostRequest);
        try
        {
            if (createPostRequest == null || string.IsNullOrEmpty(createPostRequest.Title))
            {
                return this.BadRequest("Invalid post request.");
            }

            var result = await this.postGenerator.GeneratePost(createPostRequest);

            return this.Ok(result);
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Invalid arguments provided while generating post by title: {Title}", createPostRequest.Title);
            return this.BadRequest("Invalid arguments provided.");
        }
    }

    private static void ValidatePostForGeneratingParameters(CreatePostOnlyTitleRequestDto createPostOnlyTitleRequest)
    {
        ArgumentNullException.ThrowIfNull(createPostOnlyTitleRequest);
    }
}
