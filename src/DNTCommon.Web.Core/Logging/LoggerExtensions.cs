using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Using LoggerMessage.Define to provide faster logging methods
/// </summary>
public static class LoggerExtensions
{
    private static readonly Action<ILogger, string, Exception?> LogError = LoggerMessage.Define<string>(
         LogLevel.Error,
         new EventId(1, nameof(LogErrorMessage)),
         "{Error}");

    private static readonly Action<ILogger, string, Exception?> LogInformation = LoggerMessage.Define<string>(
         LogLevel.Information,
         new EventId(1, nameof(LogInformationMessage)),
         "{Info}");

    private static readonly Action<ILogger, string, Exception?> LogDebug = LoggerMessage.Define<string>(
         LogLevel.Debug,
         new EventId(2, nameof(LogDebugMessage)),
         "{Info}");

    private static readonly Action<ILogger, string, Exception?> LogWarning = LoggerMessage.Define<string>(
         LogLevel.Warning,
         new EventId(3, nameof(LogWarningMessage)),
         "{Info}");

    /// <summary>
    ///     _logError using the LoggerMessage
    /// </summary>
    public static void LogErrorMessage(this ILogger logger, string error)
    {
        LogError(logger, error, null);
    }

    /// <summary>
    ///     _logInformation using the LoggerMessage
    /// </summary>
    public static void LogInformationMessage(this ILogger logger, string info)
    {
        LogInformation(logger, info, null);
    }

    /// <summary>
    ///     _logDebug using the LoggerMessage
    /// </summary>
    public static void LogDebugMessage(this ILogger logger, string info)
    {
        LogDebug(logger, info, null);
    }

    /// <summary>
    ///     _logWarning using the LoggerMessage
    /// </summary>
    public static void LogWarningMessage(this ILogger logger, string info)
    {
        LogWarning(logger, info, null);
    }
}