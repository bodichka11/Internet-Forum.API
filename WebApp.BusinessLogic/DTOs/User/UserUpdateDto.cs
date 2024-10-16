namespace WebApp.BusinessLogic.DTOs.User;
public class UserUpdateDto
{
    public long Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;
}
