using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WebApp.BusinessLogic.DTOs.User;
using WebApp.BusinessLogic.Helpers;
using WebApp.BusinessLogic.Models;
using WebApp.BusinessLogic.Services.Interfaces;
using WebApp.DataAccess.Entities;
using WebApp.DataAccess.Helpers;
using WebApp.DataAccess.Repositories.Interfaces;

namespace WebApp.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenService tokenService;
        private readonly string pepper;
        private readonly IMapper mapper;
        private readonly ILogger<UserService> logger;
        private readonly IWebHostEnvironment environment;

        public UserService(IUserRepository userRepository, ITokenService tokenService, IMapper mapper, ILogger<UserService> logger, IWebHostEnvironment environment)
        {
            var pepperEnv = Environment.GetEnvironmentVariable("PasswordHashExamplePepper");

            if (string.IsNullOrEmpty(pepperEnv))
            {
                throw new InvalidOperationException("Secret key environment variable is not set.");
            }

            this.userRepository = userRepository;
            this.tokenService = tokenService;
            this.mapper = mapper;
            this.pepper = pepperEnv;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task<AuthenticatedResponse> Login(UserLogin userLogin, CancellationToken cancellationToken)
        {
            ValidateLoginParameters(userLogin);

            var user = await this.userRepository.GetUserByUsernameAsync(userLogin.Username, cancellationToken);

            if (user == null || PasswordHasher.ComputeHash(userLogin.Password, user.PasswordSalt, this.pepper) != user.PasswordHash)
            {
                UserServiceLogging.LoginFailed(this.logger, userLogin.Username);
                throw new UnauthorizedAccessException("Username or password did not match.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Name, userLogin.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };
            var accessToken = this.tokenService.GenerateAccessToken(claims);
            var refreshToken = this.tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            var passwordHash = PasswordHasher.ComputeHash(userLogin.Password, user.PasswordSalt, this.pepper);
            if (user.PasswordHash != passwordHash)
            {
                throw new UnauthorizedAccessException("Username or password did not match.");
            }

            await this.userRepository.SaveChangesAsync(cancellationToken);
            UserServiceLogging.LoginSuccessfull(this.logger, userLogin.Username);

            return new AuthenticatedResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        public async Task<RegisteredResponse> Register(UserRegister userRegister, CancellationToken cancellationToken)
        {
            var user = new User()
            {
                Username = userRegister?.Username!,
                EmailAddress = userRegister?.Email!,
                PasswordSalt = PasswordHasher.GenerateSalt(),
            };
            user.PasswordHash = PasswordHasher.ComputeHash(userRegister?.Password!, user.PasswordSalt, this.pepper);
            await this.userRepository.AddUserAsync(user, cancellationToken);
            await this.userRepository.SaveChangesAsync(cancellationToken);

            UserServiceLogging.UserRegistered(this.logger, user.Username);

            return new RegisteredResponse(user.Id, user.Username, user.EmailAddress);
        }

        public async Task<UserDto> GetUser(long id)
        {
            var user = await this.userRepository.GetUserByIdAsync(id, CancellationToken.None);
            return this.mapper.Map<UserDto>(user);
        }

        public async Task<AuthenticatedResponse> RefreshToken(string token, string refreshToken)
        {
            var principal = this.tokenService.GetPrincipalFromExpiredToken(token);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ValidateTokenParameters(userId);

            var user = await this.userRepository.GetUserByIdAsync(long.Parse(userId!, CultureInfo.InvariantCulture), CancellationToken.None);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                UserServiceLogging.RefreshTokenFailed(this.logger, user?.Username ?? "Unknown");
                throw new SecurityTokenException("Invalid refresh token");
            }

            var newJwtToken = this.tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = this.tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await this.userRepository.SaveChangesAsync(CancellationToken.None);

            UserServiceLogging.RefreshTokenIssued(this.logger, user.Username);

            return new AuthenticatedResponse
            {
                AccessToken = newJwtToken,
                RefreshToken = newRefreshToken,
            };
        }

        public async Task UpdateUserAsync(UserUpdateDto userUpdateDto, CancellationToken cancellationToken)
        {
            ValidateUpdateUserParameters(userUpdateDto);
            var user = await this.userRepository.GetUserByIdAsync(userUpdateDto.Id, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            user.Username = userUpdateDto.Username;
            user.EmailAddress = userUpdateDto.EmailAddress;

            await this.userRepository.UpdateUserAsync(user, cancellationToken);
        }

        public async Task<UserDto> GetUserByUsername(string username, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.GetUserByUsernameAsync(username, cancellationToken);

            ValidateUserParameteres(user);

            return this.mapper.Map<UserDto>(user);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await this.userRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<string> UploadAvatarAsync(long userId, Stream avatarStream, string fileName, CancellationToken cancellationToken)
        {
            ValidateUploadAvatarParameters(avatarStream);

            var user = await this.userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

            if (string.IsNullOrEmpty(this.environment.WebRootPath))
            {
                throw new InvalidOperationException($"WebRootPath is not set. ContentRootPath: {this.environment.ContentRootPath}");
            }

            var uploadsFolder = Path.Combine(this.environment.WebRootPath, "avatars");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            _ = Directory.CreateDirectory(uploadsFolder);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await avatarStream.CopyToAsync(fileStream, cancellationToken);
            }

            user.ImageUrl = $"/avatars/{uniqueFileName}";
            await this.userRepository.SaveChangesAsync(cancellationToken);

            return user.ImageUrl;
        }

        private static void ValidateLoginParameters(UserLogin userLogin)
        {
            ArgumentNullException.ThrowIfNull(userLogin);
        }

        private static void ValidateTokenParameters(string? userId)
        {
            ArgumentNullException.ThrowIfNull(userId);
        }

        private static void ValidateUpdateUserParameters(UserUpdateDto userUpdateDto)
        {
            ArgumentNullException.ThrowIfNull(userUpdateDto);
        }

        private static void ValidateUserParameteres(User? user)
        {
            ArgumentNullException.ThrowIfNull(user);
        }

        private static void ValidateUploadAvatarParameters(Stream avatarStream)
        {
            ArgumentNullException.ThrowIfNull(avatarStream);
        }
    }
}
