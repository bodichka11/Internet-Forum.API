using WebApp.BusinessLogic.DTOs.Tag;

namespace WebApp.BusinessLogic.DTOs.Post;
public class CreatePostDto
{
    public long UserId { get; set; }

    public long TopicId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable IDE0028 // Simplify collection initialization
    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore CA2227 // Collection properties should be read only
}
