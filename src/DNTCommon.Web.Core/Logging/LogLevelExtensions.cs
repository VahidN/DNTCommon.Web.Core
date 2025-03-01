using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     LogLevel utils
/// </summary>
public static class LogLevelExtensions
{
    /// <summary>
    ///     Tries to convert the value to LogLevel
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LogLevel? ToLogLevel(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Enum.TryParse<LogLevel>(value, ignoreCase: true, out var logLevelValue) ? logLevelValue : null;
    }

    /// <summary>
    ///     Creates a bootstrap's class, equivalent of the given logLevel
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public static string LogLevelToBootstrapClass(this string? logLevel)
    {
        var logLevelValue = ToLogLevel(logLevel);

        if (!logLevelValue.HasValue)
        {
            return "bg-info text-light";
        }

        return logLevelValue.Value switch
        {
            LogLevel.Critical or LogLevel.Error => "bg-danger text-light",
            LogLevel.Warning => "bg-warning text-light",
            _ => "bg-info text-light"
        };
    }

    /// <summary>
    ///     Creates a bootstrap's class, based on the element's logLevel and current page's log level
    /// </summary>
    /// <param name="buttonLogLevel"></param>
    /// <param name="currentLogLevelValue"></param>
    /// <returns></returns>
    public static string GetActiveLogLevelClass(this LogLevel? buttonLogLevel, LogLevel? currentLogLevelValue)
    {
        if (buttonLogLevel is null && currentLogLevelValue is null)
        {
            return "btn btn-info btn-sm";
        }

        if (currentLogLevelValue is null)
        {
            return "btn btn-secondary btn-sm";
        }

        return currentLogLevelValue.Value switch
        {
            LogLevel.Critical or LogLevel.Error => buttonLogLevel == currentLogLevelValue.Value
                ? "btn btn-danger btn-sm"
                : "btn btn-secondary btn-sm",
            LogLevel.Warning => buttonLogLevel == currentLogLevelValue.Value
                ? "btn btn-warning btn-sm"
                : "btn btn-secondary btn-sm",
            _ => buttonLogLevel == currentLogLevelValue.Value ? "btn btn-info btn-sm" : "btn btn-secondary btn-sm"
        };
    }
}
