using WebApp.BusinessLogic.DTOs.Post;

namespace WebApp.BusinessLogic.DTOs.Topic;
public class TopicDto
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable IDE0028 // Simplify collection initialization
    public ICollection<PostDto> Posts { get; set; } = new List<PostDto>();
#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore CA2227 // Collection properties should be read only
}
