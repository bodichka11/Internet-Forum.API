using WebApp.DataAccess.Entities.Base;

namespace WebApp.DataAccess.Entities;
public class Topic : Entity<long>
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable IDE0028 // Simplify collection initialization
    public ICollection<Post> Posts { get; set; } = new List<Post>();
#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore CA2227 // Collection properties should be read only
}
