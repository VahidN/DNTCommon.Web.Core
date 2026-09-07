using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

public class FileLoggerOptions
{
    public FileLoggerOptions() { }

    public FileLoggerOptions(string logsDirectoryPath, string secretKey, LogLevel minLevel = LogLevel.Warning)
    {
        ArgumentNullException.ThrowIfNull(logsDirectoryPath);
        ArgumentNullException.ThrowIfNull(secretKey);

        LogsDirectoryPath = logsDirectoryPath;
        SecretKey = secretKey;

        MinLevel = minLevel;
    }

    /// <summary>
    ///     Its default value is Warning
    /// </summary>
    public LogLevel MinLevel { get; set; }

    public string? LogsDirectoryPath { get; set; }

    public string? SecretKey { get; set; }
}
