namespace WebApp.WebApi.Interfaces;

public interface IOpenAiSettings
{
    public string GetPostPrompt(string title);
}
