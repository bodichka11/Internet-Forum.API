using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.BusinessLogic.Services.Interfaces;

namespace WebApp.WebApi.Controllers;

/// <summary>
/// Controller for handling tag-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService tagService;
    private readonly ILogger<TagController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagController"/> class.
    /// </summary>
    /// <param name="tagService">The tag service.</param>
    /// <param name="logger">The logger instance.</param>
    public TagController(ITagService tagService, ILogger<TagController> logger)
    {
        this.tagService = tagService;
        this.logger = logger;
    }

    /// <summary>
    /// Adds tags to a specific post.
    /// </summary>
    /// <param name="postId">The post ID.</param>
    /// <param name="tagNames">The collection of tag names to be added.</param>
    /// <returns>An action result indicating success or failure.</returns>
    [HttpPost("{postId}/tags")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> AddTagsToPost(long postId, [FromBody] ICollection<string> tagNames)
    {
        if (tagNames == null || tagNames.Count == 0)
        {
            this.logger.LogWarning("AddTagsToPost called with null or empty tagNames for postId {PostId}", postId);
            return this.BadRequest("At least one tag name is required.");
        }

        try
        {
            this.logger.LogInformation("Adding tags to post {PostId}", postId);
            await this.tagService.AddTagsToPostAsync(postId, tagNames);
            return this.Ok("Tags added to the post.");
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Invalid arguments provided while adding tags to post {PostId}", postId);
            return this.BadRequest("Invalid arguments provided.");
        }
    }
}
