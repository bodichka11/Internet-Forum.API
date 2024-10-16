using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Helpers;

namespace WebApp.DataAccess.DataContext;
public class ForumDbContext : DbContext
{
    public ForumDbContext(DbContextOptions<ForumDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => this.Set<User>();

    public DbSet<Post> Posts => this.Set<Post>();

    public DbSet<Comment> Comments => this.Set<Comment>();

    public DbSet<Reaction> Reactions => this.Set<Reaction>();

    public DbSet<Topic> Topics => this.Set<Topic>();

    public DbSet<Tag> Tags => this.Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        _ = modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        _ = modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        _ = modelBuilder.Entity<Post>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.Posts)
            .UsingEntity(j => j.ToTable("PostTags"));

        _ = modelBuilder.Entity<Reaction>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reactions)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        _ = modelBuilder.Entity<Reaction>()
            .HasOne(r => r.Post)
            .WithMany(p => p.Reactions)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        _ = modelBuilder.Entity<Reaction>()
            .HasOne(r => r.Comment)
            .WithMany(c => c.Reactions)
            .HasForeignKey(r => r.CommentId)
            .OnDelete(DeleteBehavior.NoAction);

        var adminSalt = PasswordHasher.GenerateSalt();
        _ = modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "administrator",
                EmailAddress = "admin@qwerty.com",
                PasswordSalt = adminSalt,
                PasswordHash = PasswordHasher.ComputeHash("administrator", adminSalt, "<YOUR HASH PEPPER>"),
                Role = Entities.Enums.UserRole.Admin,
            });

        _ = modelBuilder.Entity<Topic>().HasData(
                new Topic { Id = 1, Name = "Sport" },
                new Topic { Id = 2, Name = "Technology" },
                new Topic { Id = 3, Name = "Self-Development" },
                new Topic { Id = 4, Name = "Health & Wellness" },
                new Topic { Id = 5, Name = "Finance" },
                new Topic { Id = 6, Name = "Education" },
                new Topic { Id = 7, Name = "Travel" },
                new Topic { Id = 8, Name = "Productivity" },
                new Topic { Id = 9, Name = "Books & Literature" },
                new Topic { Id = 10, Name = "Entertainment" });

        base.OnModelCreating(modelBuilder);
    }
}
