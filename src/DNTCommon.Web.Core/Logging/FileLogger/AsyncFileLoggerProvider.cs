using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

public sealed class AsyncFileLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, AsyncFileLogger> _loggers = new(StringComparer.Ordinal);
    private readonly FileLoggerOptions _options;
    private readonly LogQueueProcessor _processor;

    public AsyncFileLoggerProvider(IBackgroundQueueService backgroundQueueService,
        IOptions<FileLoggerOptions> fileLoggerOptions)
    {
        ArgumentNullException.ThrowIfNull(backgroundQueueService);
        ArgumentNullException.ThrowIfNull(fileLoggerOptions);

        _options = fileLoggerOptions.Value;
        _processor = new LogQueueProcessor(backgroundQueueService, _options);
    }

    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName,
            static (name, provider) => new AsyncFileLogger(name, provider._processor, provider._options), this);

    public void Dispose() => _loggers.Clear();
}
