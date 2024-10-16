namespace WebApp.BusinessLogic.Models;
public class RegisteredResponse
{
    public RegisteredResponse(long id, string username, string email)
    {
        this.Id = id;
        this.Username = username;
        this.Email = email;
    }

    public long Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }
}
