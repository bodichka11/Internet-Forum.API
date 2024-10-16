using Microsoft.Extensions.Logging;

namespace WebApp.BusinessLogic.Helpers;
public static class TopicServiceLogging
{
    private static readonly Action<ILogger, string, Exception?> TopicCreatedValue =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(TopicCreated)),
            "Post with title {Title} created successfully");

    private static readonly Action<ILogger, string, Exception?> TopicDeletedValue =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(2, nameof(TopicDeleted)),
            "Post with title {Title} deleted successfully");

    private static readonly Action<ILogger, string, Exception?> TopicUpdatedValue =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(3, nameof(TopicUpdated)),
            "Post with title {Title} updated successfully");

    private static readonly Action<ILogger, string, Exception?> TopicActionFailedValue =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(4, nameof(TopicActionFailed)),
            "Action on post with title {Title} failed");

    public static void TopicCreated(ILogger logger, string title)
    {
        TopicCreatedValue(logger, title, null);
    }

    public static void TopicDeleted(ILogger logger, string title)
    {
        TopicDeletedValue(logger, title, null);
    }

    public static void TopicUpdated(ILogger logger, string title)
    {
        TopicUpdatedValue(logger, title, null);
    }

    public static void TopicActionFailed(ILogger logger, string title, Exception? exception = null)
    {
        TopicActionFailedValue(logger, title, exception);
    }
}
