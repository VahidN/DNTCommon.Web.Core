using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

public class FileLoggerOptions
{
    public FileLoggerOptions() { }

    public FileLoggerOptions(string logsDirectoryPath, LogLevel minLevel = LogLevel.Warning)
    {
        ArgumentNullException.ThrowIfNull(logsDirectoryPath);
        LogsDirectoryPath = logsDirectoryPath;
        MinLevel = minLevel;
    }

    /// <summary>
    ///     Its default value is Warning
    /// </summary>
    public LogLevel MinLevel { get; set; }

    public string? LogsDirectoryPath { get; set; }
}
