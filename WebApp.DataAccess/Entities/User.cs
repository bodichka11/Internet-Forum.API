using WebApp.DataAccess.Entities.Base;
using WebApp.DataAccess.Entities.Enums;

namespace WebApp.DataAccess.Entities
{
    public class User : Entity<long>
    {
        public string Username { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string PasswordSalt { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

#pragma warning disable CA1056 // URI-like properties should not be strings
        public string? ImageUrl { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

        public UserRole Role { get; set; } = UserRole.User;

        public DateTime RefreshTokenExpiryTime { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable IDE0028 // Simplify collection initialization
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
