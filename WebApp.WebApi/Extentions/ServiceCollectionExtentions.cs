using OpenAI_API;
using WebApp.WebApi.AiSettings;
using WebApp.WebApi.Interfaces;
using WebApp.WebApi.Services;

namespace WebApp.WebApi.Extentions;

public static class ServiceCollectionExtentions
{
    public static void RegisterOpenAI(this IServiceCollection services, IConfiguration configuration)
    {
        var openAiApiKey = configuration.GetValue<string>("OpenAI_API_KEY");

        ArgumentNullException.ThrowIfNull(openAiApiKey);

        _ = services.AddSingleton(e => new OpenAIAPI(new APIAuthentication(openAiApiKey)));
        _ = services.AddScoped<IOpenAiSettings, OpenAiSettings>();
        _ = services.AddScoped<ICompletionService, CompletionService>();
        _ = services.AddScoped<IPostGenerator, PostGenerator>();
    }
}
