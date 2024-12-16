using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Chromium Utils
/// </summary>
public static class ChromiumUtils
{
    /// <summary>
    ///     If the given `log` contains error or warning keywords, it will be logged.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="logger"></param>
    public static void LogPossibleErrorsOrWarnings(this string? log, ILogger logger)
    {
        if (log.IsEmpty())
        {
            return;
        }

        string[] keywords = ["error", "warning"];

        if (keywords.Any(keyword => log.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
        {
            logger.LogWarningMessage(log);
        }
    }
}