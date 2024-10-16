using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApp.DataAccess.DataContext;
using WebApp.DataAccess.Repositories;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.DataAccess;
public static class DalDependencyInjection
{
    public static void RegisterDALDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddDbContext<ForumDbContext>(options =>
        {
            _ = options.UseSqlServer(configuration.GetConnectionString("DbConnection"));
        });

        _ = services.AddScoped<IUserRepository, UserRepository>();
        _ = services.AddScoped<IPostRepository, PostRepository>();
        _ = services.AddScoped<ITopicRepository, TopicRepository>();
        _ = services.AddScoped<ICommentRepository, CommentRepository>();
        _ = services.AddScoped<IReactionRepository, ReactionRepository>();
        _ = services.AddScoped<ITagRepository, TagRepository>();
    }
}
