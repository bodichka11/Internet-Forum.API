namespace WebApp.BusinessLogic.DTOs.Comment;
public class CommentCreateDto
{
    public string Content { get; set; } = string.Empty;

    public long UserId { get; set; }

    public long PostId { get; set; }
}
