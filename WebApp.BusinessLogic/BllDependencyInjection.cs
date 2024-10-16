using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using WebApp.BusinessLogic.MappingProfiles;
using WebApp.BusinessLogic.Services;
using WebApp.BusinessLogic.Services.Interfaces;

namespace WebApp.BusinessLogic;
public static class BllDependencyInjection
{
    public static void RegisterBllDependencies(this IServiceCollection services)
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new UserProfile());
            mc.AddProfile(new PostProfile());
            mc.AddProfile(new TopicProfile());
            mc.AddProfile(new CommentProfile());
            mc.AddProfile(new ReactionProfile());
            mc.AddProfile(new TagProfile());
        });

        var mapper = mapperConfig.CreateMapper();
        _ = services.AddSingleton(mapper);

        _ = services.AddScoped<IUserService, UserService>();
        _ = services.AddTransient<ITokenService, TokenService>();
        _ = services.AddScoped<IPostService, PostService>();
        _ = services.AddScoped<ITopicService, TopicService>();
        _ = services.AddScoped<ICommentService, CommentService>();
        _ = services.AddScoped<IReactionService, ReactionService>();
        _ = services.AddScoped<ITagService, TagService>();
    }
}
