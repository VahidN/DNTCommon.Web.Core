using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

public sealed class AsyncFileLogger(string categoryName, LogQueueProcessor processor, FileLoggerOptions options)
    : ILogger
{
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= options.MinLevel;

    public void Log<TState>(LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);

        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);

        if (message.IsEmpty())
        {
            return;
        }

        var timestamp = DateTime.UtcNow.ToString(format: "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        var logEntry = $"[{timestamp}] [{logLevel}] [{categoryName}]{Environment.NewLine}{message}";

        if (exception is not null)
        {
            logEntry += $"{Environment.NewLine}{exception.Demystify()}";
        }

        processor.EnqueueLog(logEntry + Environment.NewLine);
    }
}
