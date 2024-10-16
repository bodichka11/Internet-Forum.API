using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApp.BusinessLogic.Models;
using WebApp.BusinessLogic.Services.Interfaces;

namespace WebApp.WebApi.Controllers;

/// <summary>
/// Controller for handling token-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ILogger<TokenController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenController"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    /// <param name="logger">The logger instance.</param>
    public TokenController(IUserService userService, ILogger<TokenController> logger)
    {
        this.userService = userService;
        this.logger = logger;
    }

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    /// <param name="request">The token request containing the access token and refresh token.</param>
    /// <returns>The refreshed token response.</returns>
    [HttpPost("refresh")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> Refresh([FromBody] Token request)
    {
        if (request == null || string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
        {
            this.logger.LogWarning("Refresh called with invalid client request");
            return this.BadRequest("Invalid client request");
        }

        try
        {
            this.logger.LogInformation("Attempting to refresh token for accessToken: {AccessToken}", request.AccessToken);
            var response = await this.userService.RefreshToken(request.AccessToken, request.RefreshToken);
            return this.Ok(response);
        }
        catch (SecurityTokenException ex)
        {
            this.logger.LogError(ex, "Invalid refresh token provided for accessToken: {AccessToken}", request.AccessToken);
            return this.Unauthorized("Invalid refresh token");
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Invalid argument provided while refreshing token");
            return this.BadRequest("Invalid arguments provided.");
        }
    }

    /// <summary>
    /// Revokes the current user's refresh token.
    /// </summary>
    /// <returns>No content if successful.</returns>
    [HttpPost("revoke")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> Revoke()
    {
        var username = this.User?.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            this.logger.LogWarning("Revoke called with unauthorized request");
            return this.Unauthorized();
        }

        try
        {
            var user = await this.userService.GetUserByUsername(username, CancellationToken.None);
            if (user == null)
            {
                this.logger.LogWarning("User {Username} not found", username);
                return this.NotFound("User not found");
            }

            user.RefreshToken = null!;
            user.RefreshTokenExpiryTime = DateTime.Now;
            await this.userService.SaveChangesAsync(CancellationToken.None);
            this.logger.LogInformation("Token successfully revoked for user {Username}", username);
            return this.NoContent();
        }
        catch (ArgumentException ex)
        {
            this.logger.LogError(ex, "Invalid argument provided while revoking token for user {Username}", username);
            return this.BadRequest("Invalid arguments provided.");
        }
    }
}
