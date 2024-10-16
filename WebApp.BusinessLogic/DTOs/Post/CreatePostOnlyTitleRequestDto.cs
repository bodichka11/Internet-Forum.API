using System.ComponentModel.DataAnnotations;

namespace WebApp.BusinessLogic.DTOs.Post;
public class CreatePostOnlyTitleRequestDto
{
    [Required]
    [StringLength(100, ErrorMessage = "Title can't be longer than 100 characters.")]
    public string Title { get; set; } = string.Empty;
}
