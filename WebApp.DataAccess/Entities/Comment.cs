using WebApp.DataAccess.Entities.Base;

namespace WebApp.DataAccess.Entities;
public class Comment : Entity<long>
{
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public long UserId { get; set; }

    public User User { get; set; } = default!;

    public long PostId { get; set; }

    public Post Post { get; set; } = default!;

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable IDE0028 // Simplify collection initialization
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore CA2227 // Collection properties should be read only
}
