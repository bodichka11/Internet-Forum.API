using Microsoft.Extensions.Logging;

namespace WebApp.BusinessLogic.Helpers;
public static class UserServiceLogging
{
    private static readonly Action<ILogger, string, Exception?> LoginFailedValue =
       LoggerMessage.Define<string>(
           LogLevel.Warning,
           new EventId(1, nameof(LoginFailed)),
           "Login failed for user {Username}");

    private static readonly Action<ILogger, string, Exception?> LoginSuccessfullValue =
      LoggerMessage.Define<string>(
          LogLevel.Information,
          new EventId(1, nameof(LoginFailed)),
          "User {Username} logged in successfully");

    private static readonly Action<ILogger, string, Exception?> UserRegisteredValue =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(2, nameof(UserRegistered)),
            "User {Username} has registered successfully");

    private static readonly Action<ILogger, string, Exception?> RefreshTokenIssuedValue =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(3, nameof(RefreshTokenIssued)),
            "Refresh token issued for user {Username}");

    private static readonly Action<ILogger, string, Exception?> RefreshTokenFailedValue =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(4, nameof(RefreshTokenFailed)),
            "Refresh token failed for user {Username}");

    public static void LoginFailed(ILogger logger, string username, Exception? exception = null)
    {
        LoginFailedValue(logger, username, exception);
    }

    public static void LoginSuccessfull(ILogger logger, string username)
    {
        LoginSuccessfullValue(logger, username, null);
    }

    public static void UserRegistered(ILogger logger, string username)
    {
        UserRegisteredValue(logger, username, null);
    }

    public static void RefreshTokenIssued(ILogger logger, string username)
    {
        RefreshTokenIssuedValue(logger, username, null);
    }

    public static void RefreshTokenFailed(ILogger logger, string username)
    {
        RefreshTokenFailedValue(logger, username, null);
    }
}
