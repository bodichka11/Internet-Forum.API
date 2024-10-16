using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.BusinessLogic.DTOs.User;
using WebApp.BusinessLogic.Models;
using WebApp.BusinessLogic.Services.Interfaces;

namespace WebApp.WebApi.Controllers;

/// <summary>
/// Controller for handling user-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ILogger<UserController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    /// <param name="logger">The logger instance.</param>
    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        this.userService = userService;
        this.logger = logger;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="userRegister">The user registration details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response object after registering the user.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserRegister userRegister, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Registering user");
        var response = await this.userService.Register(userRegister, cancellationToken);
        return this.Ok(response);
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="userLogin">The user login details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response object after logging in the user.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin, CancellationToken cancellationToken)
    {
        if (userLogin is null)
        {
            this.logger.LogWarning("Login called with null UserLogin");
            return this.BadRequest("Invalid client request");
        }

        try
        {
            this.logger.LogInformation("Logging in user");
            var response = await this.userService.Login(userLogin, cancellationToken);
            return this.Ok(response);
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Argument error occurred while logging in user");
            return this.BadRequest("Invalid arguments provided.");
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogError(ex, "Invalid operation error occurred while logging in user");
            return this.BadRequest("Operation not allowed.");
        }
        catch (UnauthorizedAccessException ex)
        {
            this.logger.LogError(ex, "Unauthorized access error occurred while logging in user");
            return this.Unauthorized("Invalid credentials provided.");
        }
    }

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>The user details if found.</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> GetUserById([FromRoute] long id)
    {
        this.logger.LogInformation("Fetching user by id {UserId}", id);

        var response = await this.userService.GetUser(id);
        if (response == null)
        {
            this.logger.LogWarning("User not found with id {UserId}", id);
            return this.NotFound();
        }

        return this.Ok(response);
    }

    /// <summary>
    /// Updates a user's information.
    /// </summary>
    /// <param name="userUpdateDto">The user update details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An action result indicating the success of the operation.</returns>
    [HttpPut("specific-user")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateSpecificUser([FromBody] UserUpdateDto userUpdateDto, CancellationToken cancellationToken)
    {
        if (userUpdateDto == null)
        {
            this.logger.LogWarning("UpdateUser called with null UserUpdateDto");
            return this.BadRequest();
        }

        this.logger.LogInformation("Updating user");

        try
        {
            await this.userService.UpdateUserAsync(userUpdateDto, cancellationToken);
            return this.Ok();
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Argument error occurred while updating user");
            return this.BadRequest("Invalid arguments provided.");
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogError(ex, "Invalid operation error occurred while updating user");
            return this.BadRequest("Operation not allowed.");
        }
    }

    /// <summary>
    /// Updates a user's information.
    /// </summary>
    /// <param name="currentUserUpdateDto">The user update details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An action result indicating the success of the operation.</returns>
    [HttpPut]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] CurrentUserUpdateDto currentUserUpdateDto, CancellationToken cancellationToken)
    {
        var userIdentifierClaim = this.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier);

        if (userIdentifierClaim == null)
        {
            this.logger.LogWarning("AddComment called without user identifier");
            return this.Unauthorized();
        }

        if (currentUserUpdateDto == null)
        {
            this.logger.LogWarning("UpdateCurrentUser called with null UserUpdateDto");
            return this.BadRequest();
        }

        this.logger.LogInformation("Updating current user");

        try
        {
            var userUpdateDto = new UserUpdateDto()
            {
                Id = long.Parse(userIdentifierClaim.Value, CultureInfo.InvariantCulture),
                Username = currentUserUpdateDto.Username,
                EmailAddress = currentUserUpdateDto.EmailAddress,
            };
            await this.userService.UpdateUserAsync(userUpdateDto, cancellationToken);
            return this.Ok();
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Argument error occurred while updating user");
            return this.BadRequest("Invalid arguments provided.");
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogError(ex, "Invalid operation error occurred while updating user");
            return this.BadRequest("Operation not allowed.");
        }
    }

    /// <summary>
    /// Gets the currently authenticated user's details.
    /// </summary>
    /// <returns>The current user's details.</returns>
    [HttpGet("current")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            this.logger.LogWarning("GetCurrentUser called with no authorized user");
            return this.Unauthorized();
        }

        this.logger.LogInformation("Fetching current user");

        var response = await this.userService.GetUser(long.Parse(userId, CultureInfo.InvariantCulture));
        return this.Ok(response);
    }

    /// <summary>
    /// Uploads an avatar for the currently authenticated user.
    /// </summary>
    /// <param name="file">The avatar file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The URL of the uploaded avatar.</returns>
    [HttpPost("avatar")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            this.logger.LogWarning("UploadAvatar called with no file uploaded");
            return this.BadRequest("No file uploaded");
        }

        var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            this.logger.LogWarning("UploadAvatar called with no authorized user");
            return this.Unauthorized();
        }

        this.logger.LogInformation("Uploading avatar for user {UserId}", userId);

        try
        {
            using var stream = file.OpenReadStream();
            var avatarUrl = await this.userService.UploadAvatarAsync(long.Parse(userId, CultureInfo.InvariantCulture), stream, file.FileName, cancellationToken);
            return this.Ok(new { avatarUrl });
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Argument error occurred while uploading avatar for user {UserId}", userId);
            return this.BadRequest("Invalid arguments provided.");
        }
        catch (InvalidOperationException ex)
        {
            this.logger.LogError(ex, "Invalid operation error occurred while uploading avatar for user {UserId}", userId);
            return this.BadRequest("Operation not allowed.");
        }
    }
}
