namespace WebApp.BusinessLogic.Models;
public class AuthenticatedResponse
{
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }
}
