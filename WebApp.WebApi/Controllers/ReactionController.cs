using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.BusinessLogic.DTOs.Reaction;
using WebApp.BusinessLogic.Services.Interfaces;

namespace WebApp.WebApi.Controllers;

/// <summary>
/// Controller for handling reaction-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReactionController : ControllerBase
{
    private readonly IReactionService reactionService;
    private readonly ILogger<ReactionController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactionController"/> class.
    /// </summary>
    /// <param name="reactionService">The reaction service.</param>
    /// <param name="logger">The logger instance.</param>
    public ReactionController(IReactionService reactionService, ILogger<ReactionController> logger)
    {
        this.reactionService = reactionService;
        this.logger = logger;
    }

    /// <summary>
    /// Toggles a reaction to a post or comment.
    /// </summary>
    /// <param name="reactionDto">The reaction DTO.</param>
    /// <returns>An action result indicating success or failure.</returns>
    [HttpPost("toggle")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> React([FromBody] ReactionDto reactionDto)
    {
        if (reactionDto == null)
        {
            this.logger.LogWarning("React called with null DTO");
            return this.BadRequest("Reaction data is required.");
        }

        if (reactionDto.PostId == null && reactionDto.CommentId == null)
        {
            this.logger.LogWarning("React called without PostId or CommentId");
            return this.BadRequest("PostId or CommentId must be provided.");
        }

        var userIdentifierClaim = this.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier);

        if (userIdentifierClaim == null)
        {
            this.logger.LogWarning("React called without user identifier");
            return this.Unauthorized();
        }

        try
        {
            var userId = long.Parse(userIdentifierClaim.Value, CultureInfo.InvariantCulture);
            reactionDto.UserId = userId;

            this.logger.LogInformation("Reacting to post/comment by user {UserId}", userId);
            await this.reactionService.ReactAsync(reactionDto);
            return this.Ok();
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Invalid arguments provided while reacting.");
            return this.BadRequest("Invalid arguments provided.");
        }
    }

    /// <summary>
    /// Gets reactions for a specific post with pagination.
    /// </summary>
    /// <param name="postId">The post ID.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A list of reactions for the given post.</returns>
    [HttpGet("post/{postId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReactionDto>>> GetReactionsForPost(long postId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return this.BadRequest("Invalid pagination parameters");
        }

        this.logger.LogInformation("Getting reactions by post id {PostId} with pagination (Page: {PageNumber}, Size: {PageSize})", postId, pageNumber, pageSize);

        var reactions = await this.reactionService.GetReactionsByPostAsync(postId, pageNumber, pageSize);
        return this.Ok(reactions);
    }

    /// <summary>
    /// Gets reactions for a specific comment with pagination.
    /// </summary>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A list of reactions for the given comment.</returns>
    [HttpGet("comment/{commentId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReactionDto>>> GetReactionsForComment(long commentId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return this.BadRequest("Invalid pagination parameters");
        }

        this.logger.LogInformation("Getting reactions by comment id {CommentId} with pagination (Page: {PageNumber}, Size: {PageSize})", commentId, pageNumber, pageSize);

        var reactions = await this.reactionService.GetReactionsByCommentAsync(commentId, pageNumber, pageSize);
        return this.Ok(reactions);
    }
}
