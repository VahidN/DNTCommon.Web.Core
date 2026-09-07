namespace DNTCommon.Web.Core;

public sealed class LogQueueProcessor
{
    private readonly IBackgroundQueueService _backgroundQueueService;
    private readonly FileLoggerOptions _fileLoggerOptions;

    public LogQueueProcessor(IBackgroundQueueService backgroundQueueService, FileLoggerOptions fileLoggerOptions)
    {
        ArgumentNullException.ThrowIfNull(backgroundQueueService);
        ArgumentNullException.ThrowIfNull(fileLoggerOptions);

        _backgroundQueueService = backgroundQueueService;
        _fileLoggerOptions = fileLoggerOptions;
        _fileLoggerOptions.LogsDirectoryPath.CheckDirExists();
    }

    public void EnqueueLog(string message)
        => _backgroundQueueService.TryQueueBackgroundWorkItem(nameof(AsyncFileLoggerProvider),
            async (cancellationToken, _) =>
            {
                try
                {
                    var today = DateTime.UtcNow.ToString(format: "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    var fileName = _fileLoggerOptions.SecretKey.IsEmpty()
                        ? $"log-{today}.txt"
                        : $"log-{today}.{_fileLoggerOptions.SecretKey.GenerateConstantDailyHash()}.txt";

                    var filePath = _fileLoggerOptions.LogsDirectoryPath.SafePathCombine(fileName);

                    if (filePath.IsEmpty())
                    {
                        return;
                    }

                    await File.AppendAllTextAsync(filePath, message + Environment.NewLine, cancellationToken);
                }
                catch (Exception ex)
                {
                    // don't throw exceptions from logger
                    WriteLine(message);
                    WriteLine(ex);
                }
            });
}
