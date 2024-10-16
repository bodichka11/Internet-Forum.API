using WebApp.BusinessLogic.DTOs.Comment;
using WebApp.BusinessLogic.DTOs.Post;
using WebApp.BusinessLogic.DTOs.Reaction;

namespace WebApp.BusinessLogic.DTOs.User;
public class UserDto
{
    public long Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime RefreshTokenExpiryTime { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable IDE0028 // Simplify collection initialization
    public ICollection<PostDto> Posts { get; set; } = new List<PostDto>();

    public ICollection<ReactionDto> Reactions { get; set; } = new List<ReactionDto>();

    public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore CA2227 // Collection properties should be read only

#pragma warning disable CA1056 // URI-like properties should not be strings
    public string? ImageUrl { get; set; } = string.Empty;
#pragma warning restore CA1056 // URI-like properties should not be strings
}
