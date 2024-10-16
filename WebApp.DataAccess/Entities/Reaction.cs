using WebApp.DataAccess.Entities.Base;
using WebApp.DataAccess.Entities.Enums;

namespace WebApp.DataAccess.Entities;
public class Reaction : Entity<long>
{
    public long UserId { get; set; }

    public long? PostId { get; set; }

    public long? CommentId { get; set; }

    public ReactionType Type { get; set; } = ReactionType.Like;

    public User User { get; set; } = default!;

    public Post? Post { get; set; }

    public Comment? Comment { get; set; }
}
