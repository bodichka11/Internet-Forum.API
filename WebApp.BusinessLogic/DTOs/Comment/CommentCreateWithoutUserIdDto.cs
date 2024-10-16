using System.ComponentModel.DataAnnotations;

namespace WebApp.BusinessLogic.DTOs.Comment;
public class CommentCreateWithoutUserIdDto
{
    [Required(ErrorMessage = "Content is required.")]
    [StringLength(5000, ErrorMessage = "Content can't be longer than 5000 characters.")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Post ID is required.")]
    public long PostId { get; set; }
}
