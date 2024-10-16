using OpenAI_API;
using OpenAI_API.Models;
using WebApp.WebApi.Interfaces;

namespace WebApp.WebApi.Services;

public class CompletionService : ICompletionService
{
    private readonly OpenAIAPI openAiApi;
    private readonly ILogger<CompletionService> logger;

    public CompletionService(OpenAIAPI api, ILogger<CompletionService> logger)
    {
        this.openAiApi = api ?? throw new ArgumentNullException(nameof(api));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> CreateCompletionAsync(string prompt)
    {
        ValidatePrompt(prompt);

        var chat = this.openAiApi.Chat.CreateConversation();
        chat.Model = Model.GPT4;

        chat.AppendUserInput(prompt);

        string? response = await chat.GetResponseFromChatbotAsync();

        if (response == null)
        {
            this.logger.LogError("Chatbot response is null");
            throw new InvalidOperationException("Chatbot response is null");
        }

        return response;
    }

    private static void ValidatePrompt(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));
        }
    }
}
