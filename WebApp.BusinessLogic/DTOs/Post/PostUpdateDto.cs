using System.ComponentModel.DataAnnotations;
using WebApp.BusinessLogic.DTOs.Tag;

namespace WebApp.BusinessLogic.DTOs.Post;
public class PostUpdateDto
{
    [Required]
    [StringLength(100, ErrorMessage = "Title can't be longer than 100 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(5000, ErrorMessage = "Content can't be longer than 5000 characters.")]
    public string Content { get; set; } = string.Empty;

    [Required]
    public long TopicId { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable IDE0028 // Simplify collection initialization
    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore CA2227 // Collection properties should be read only
}
