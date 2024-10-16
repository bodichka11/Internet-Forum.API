using System.ComponentModel.DataAnnotations;
using WebApp.BusinessLogic.DTOs.Reaction;

namespace WebApp.BusinessLogic.DTOs.Comment;
public class CommentDto
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Content is required.")]
    [StringLength(5000, ErrorMessage = "Content can't be longer than 5000 characters.")]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public long UserId { get; set; }

    public long PostId { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable IDE0028 // Simplify collection initialization
    public ICollection<ReactionDto> Reactions { get; set; } = new List<ReactionDto>();
#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore CA2227 // Collection properties should be read only
}
