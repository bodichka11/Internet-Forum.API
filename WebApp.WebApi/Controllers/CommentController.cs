using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.BusinessLogic.DTOs.Comment;
using WebApp.BusinessLogic.Services.Interfaces;

namespace WebApp.WebApi.Controllers;

/// <summary>
/// Controller for handling comment-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService commentService;
    private readonly IMapper mapper;
    private readonly ILogger<CommentController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentController"/> class.
    /// </summary>
    /// <param name="commentService">The comment service.</param>
    /// <param name="mapper">The object mapper.</param>
    /// <param name="logger">The logger instance.</param>
    public CommentController(ICommentService commentService, IMapper mapper, ILogger<CommentController> logger)
    {
        this.commentService = commentService;
        this.mapper = mapper;
        this.logger = logger;
    }

    /// <summary>
    /// Adds a new comment.
    /// </summary>
    /// <param name="commentCreateWithoutUserIdDto">The comment DTO without user ID.</param>
    /// <returns>The created comment.</returns>
    [HttpPost]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<CommentDto>> AddComment(CommentCreateWithoutUserIdDto commentCreateWithoutUserIdDto)
    {
        if (commentCreateWithoutUserIdDto == null)
        {
            this.logger.LogWarning("AddComment called with null DTO");
            return this.BadRequest();
        }

        var userIdentifierClaim = this.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier);

        if (userIdentifierClaim == null)
        {
            this.logger.LogWarning("AddComment called without user identifier");
            return this.Unauthorized();
        }

        try
        {
            var commentCreateDto = this.mapper.Map<CommentCreateDto>(commentCreateWithoutUserIdDto);
            commentCreateDto.UserId = long.Parse(userIdentifierClaim.Value, CultureInfo.InvariantCulture);

            this.logger.LogInformation("Adding comment for user {UserId}", commentCreateDto.UserId);

            var createdComment = await this.commentService.AddComment(commentCreateDto);

            if (createdComment == null || createdComment.Id <= 0)
            {
                this.logger.LogError("Invalid created comment for user {UserId}", commentCreateDto.UserId);
                return this.BadRequest("Invalid created comment.");
            }

            this.logger.LogInformation("Comment created with id {CommentId}", createdComment.Id);
            return this.Ok(createdComment);
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogError(ex, "Invalid operation occurred while adding comment.");
            return this.BadRequest("Invalid operation.");
        }
    }

    /// <summary>
    /// Gets a comment by its ID.
    /// </summary>
    /// <param name="id">The comment ID.</param>
    /// <returns>The comment if found.</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCommentByIdAsync(long id)
    {
        this.logger.LogInformation("Getting comment by id {CommentId}", id);

        var post = await this.commentService.GetCommentById(id);
        if (post == null)
        {
            this.logger.LogWarning("Comment not found with id {CommentId}", id);
            return this.NotFound();
        }

        return this.Ok(post);
    }

    /// <summary>
    /// Gets all comments for a specific post with pagination.
    /// </summary>
    /// <param name="postId">The post ID.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A list of comments.</returns>
    [HttpGet("post/{postId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCommentsByPostId(long postId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return this.BadRequest("Invalid pagination parameters");
        }

        this.logger.LogInformation("Getting comments by post id {PostId} with pagination (Page: {PageNumber}, Size: {PageSize})", postId, pageNumber, pageSize);

        var comments = await this.commentService.GetCommentsByPostId(postId, pageNumber, pageSize);
        return this.Ok(comments);
    }

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="id">The comment ID.</param>
    /// <param name="commentUpdateDto">The updated comment DTO.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> UpdateComment(long id, CommentUpdateDto commentUpdateDto)
    {
        if (commentUpdateDto == null)
        {
            this.logger.LogWarning("UpdateComment called with null DTO");
            return this.BadRequest();
        }

        this.logger.LogInformation("Updating comment with id {CommentId}", id);

        try
        {
            var existingComment = await this.commentService.GetCommentById(id);

            if (existingComment == null)
            {
                this.logger.LogWarning("Comment not found with id {CommentId}", id);
                return this.NotFound();
            }

            _ = this.mapper.Map(commentUpdateDto, existingComment);
            _ = await this.commentService.UpdateComment(existingComment);
            this.logger.LogInformation("Comment updated with id {CommentId}", id);
            return this.NoContent();
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Argument error occurred while updating comment with id {CommentId}.", id);
            return this.BadRequest("Invalid argument provided.");
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogWarning(ex, "Comment not found with id {CommentId}", id);
            return this.NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a comment by its ID.
    /// </summary>
    /// <param name="id">The comment ID.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> DeleteComment(long id)
    {
        this.logger.LogInformation("Deleting comment with id {CommentId}", id);

        try
        {
            await this.commentService.DeleteComment(id);
            this.logger.LogInformation("Comment deleted with id {CommentId}", id);
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogWarning(ex, "Comment not found with id {CommentId}", id);
            return this.NotFound(ex.Message);
        }
    }
}
