using System.ComponentModel.DataAnnotations;

namespace WebApp.BusinessLogic.DTOs.Tag;
public class TagDto
{
    [StringLength(50, ErrorMessage = "Tag name cannot be longer than 50 characters")]
    public string Name { get; set; } = string.Empty;
}
