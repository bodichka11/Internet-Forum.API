using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebApp.DataAccess.Entities.Enums;

namespace WebApp.BusinessLogic.DTOs.Reaction;
public class ReactionDto
{
    [JsonIgnore]
    public long UserId { get; set; }

    public long? PostId { get; set; }

    public long? CommentId { get; set; }

    [Required(ErrorMessage = "Reaction type is required.")]
    [EnumDataType(typeof(ReactionType), ErrorMessage = "Invalid reaction type.")]
    public ReactionType Type { get; set; }
}
