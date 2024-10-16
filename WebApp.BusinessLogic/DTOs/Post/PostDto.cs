using System.ComponentModel.DataAnnotations;
using WebApp.BusinessLogic.DTOs.Comment;
using WebApp.BusinessLogic.DTOs.Reaction;
using WebApp.BusinessLogic.DTOs.Tag;

namespace WebApp.BusinessLogic.DTOs.Post;
public class PostDto
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(5000)]
    public string Content { get; set; } = string.Empty;

    public long UserId { get; set; }

    public long TopicId { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable IDE0028 // Simplify collection initialization
    public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();

    public ICollection<ReactionDto> Reactions { get; set; } = new List<ReactionDto>();

    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();

#pragma warning disable CA1002 // Do not expose generic lists
    public List<string> Images { get; set; } = new List<string>();
#pragma warning restore CA1002 // Do not expose generic lists
#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore CA2227 // Collection properties should be read only
}
