using Microsoft.Extensions.Logging;

namespace WebApp.BusinessLogic.Helpers;
public static class PostServiceLogging
{
    private static readonly Action<ILogger, string, Exception?> PostCreatedValue =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(PostCreated)),
            "Post with title {Title} created successfully");

    private static readonly Action<ILogger, string, Exception?> PostDeletedValue =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(2, nameof(PostDeleted)),
            "Post with title {Title} deleted successfully");

    private static readonly Action<ILogger, string, Exception?> PostUpdatedValue =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(3, nameof(PostUpdated)),
            "Post with title {Title} updated successfully");

    private static readonly Action<ILogger, string, Exception?> PostActionFailedValue =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(4, nameof(PostActionFailed)),
            "Action on post with title {Title} failed");

    public static void PostCreated(ILogger logger, string title)
    {
        PostCreatedValue(logger, title, null);
    }

    public static void PostDeleted(ILogger logger, string title)
    {
        PostDeletedValue(logger, title, null);
    }

    public static void PostUpdated(ILogger logger, string title)
    {
        PostUpdatedValue(logger, title, null);
    }

    public static void PostActionFailed(ILogger logger, string title, Exception? exception = null)
    {
        PostActionFailedValue(logger, title, exception);
    }
}
