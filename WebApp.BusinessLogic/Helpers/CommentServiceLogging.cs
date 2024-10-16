using Microsoft.Extensions.Logging;

namespace WebApp.BusinessLogic.Helpers;

/// <summary>
/// Provides logging functionalities for the CommentService.
/// </summary>
public static class CommentServiceLogging
{
    private static readonly Action<ILogger, long, Exception?> CommentAddedValue =
            LoggerMessage.Define<long>(
                LogLevel.Information,
                new EventId(1, nameof(CommentAdded)),
                "Comment with ID {CommentId} added successfully");

    private static readonly Action<ILogger, long, Exception?> CommentDeletedValue =
        LoggerMessage.Define<long>(
            LogLevel.Information,
            new EventId(2, nameof(CommentDeleted)),
            "Comment with ID {CommentId} deleted successfully");

    private static readonly Action<ILogger, long, Exception?> CommentUpdatedValue =
        LoggerMessage.Define<long>(
            LogLevel.Information,
            new EventId(3, nameof(CommentUpdated)),
            "Comment with ID {CommentId} updated successfully");

    /// <summary>
    /// Logs that a comment has been added successfully.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="commentId">The ID of the added comment.</param>
    public static void CommentAdded(ILogger logger, long commentId)
    {
        CommentAddedValue(logger, commentId, null);
    }

    /// <summary>
    /// Logs that a comment has been deleted successfully.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="commentId">The ID of the deleted comment.</param>
    public static void CommentDeleted(ILogger logger, long commentId)
    {
        CommentDeletedValue(logger, commentId, null);
    }

    /// <summary>
    /// Logs that a comment has been updated successfully.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="commentId">The ID of the updated comment.</param>
    public static void CommentUpdated(ILogger logger, long commentId)
    {
        CommentUpdatedValue(logger, commentId, null);
    }
}
