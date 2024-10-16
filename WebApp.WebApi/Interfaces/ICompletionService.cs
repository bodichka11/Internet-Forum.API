namespace WebApp.WebApi.Interfaces;

public interface ICompletionService
{
    Task<string> CreateCompletionAsync(string prompt);
}
