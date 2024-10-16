using System.ComponentModel.DataAnnotations;

namespace WebApp.BusinessLogic.DTOs.Topic;
public class TopicCreateDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    public string? Description { get; set; }
}
