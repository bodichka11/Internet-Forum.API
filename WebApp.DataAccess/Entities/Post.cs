using WebApp.DataAccess.Entities.Base;

namespace WebApp.DataAccess.Entities;
public class Post : Entity<long>
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public long TopicId { get; set; }

    public Topic Topic { get; set; } = default!;

    public long UserId { get; set; }

    public User User { get; set; } = default!;

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable IDE0028 // Simplify collection initialization
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

#pragma warning disable CA1002 // Do not expose generic lists

    public List<string> Images { get; set; } = new List<string>();

    public string Link { get; set; } = string.Empty;

#pragma warning restore CA1002 // Do not expose generic lists
#pragma warning restore CA2227 // Collection properties should be read only
#pragma warning restore IDE0028 // Simplify collection initialization
}
