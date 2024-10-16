namespace WebApp.BusinessLogic.DTOs.Post;
public class OpenAiGenerateResponseDto
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public long TopicId { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable IDE0028 // Simplify collection initialization
    public ICollection<string> Tags { get; set; } = new List<string>();
#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore CA2227 // Collection properties should be read only
}
