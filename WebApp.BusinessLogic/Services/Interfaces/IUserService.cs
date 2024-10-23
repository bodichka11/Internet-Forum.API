using WebApp.BusinessLogic.DTOs.User;
using WebApp.BusinessLogic.Models;

namespace WebApp.BusinessLogic.Services.Interfaces;
public interface IUserService
{
    Task<AuthenticatedResponse> Login(UserLogin userLogin, CancellationToken cancellationToken);

    Task<RegisteredResponse> Register(UserRegister userRegister, CancellationToken cancellationToken);

    Task<UserDto> GetUser(long id);

    Task<AuthenticatedResponse> RefreshToken(string token, string refreshToken);

    Task UpdateUserAsync(UserUpdateDto userUpdateDto, CancellationToken cancellationToken);

    Task<UserDto> GetUserByUsername(string username, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<string> UploadAvatarAsync(long userId, Stream avatarStream, string fileName, CancellationToken cancellationToken);

    Task SendUpdateConfirmationEmailAsync(UpdateRequestModel model);

    Task<bool> ConfirmUpdate(string email, string code, CancellationToken cancellationToken);
}
