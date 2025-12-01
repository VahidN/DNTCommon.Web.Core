using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     DownloaderService Extensions
/// </summary>
public static class DownloaderServiceExtensions
{
    private const int
        MaxBufferSize =
            0x10000; // 64K. The artificial constraint due to win32 api limitations. Increasing the buffer size beyond 64k will not help in any circumstance, as the underlying SMB protocol does not support buffer lengths beyond 64k.

    /// <summary>
    ///     Adds IDownloaderService to IServiceCollection
    /// </summary>
    public static IServiceCollection AddDownloaderService(this IServiceCollection services)
    {
        services.AddScoped<IDownloaderService, DownloaderService>();

        return services;
    }

    /// <summary>
    ///     Downloads a file from a given url and then stores it as a local file.
    /// </summary>
    public static Task<DownloadStatus?> DownloadFileWithStatusReportAsync(this HttpClient client,
        string url,
        string outputFilePath,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default)
        => DownloadAsync(
            ct => DoDownloadFileAsync(client, url, outputFilePath, onDownloadStatusChanged, onDownloadCompleted, ct),
            autoRetries, cancellationToken);

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a byte array.
    /// </summary>
    public static Task<(byte[] Data, DownloadStatus DownloadStatus)> DownloadDataWithStatusReportAsync(
        this HttpClient client,
        string url,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default)
        => DownloadAsync(ct => DoDownloadDataAsync(client, url, onDownloadCompleted, onDownloadStatusChanged, ct),
            autoRetries, cancellationToken);

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a text.
    /// </summary>
    public static async Task<(string Data, DownloadStatus DownloadStatus)> DownloadPageWithStatusReportAsync(
        this HttpClient client,
        string url,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default)
    {
        var (data, downloadStatus) = await DownloadDataWithStatusReportAsync(client, url, autoRetries,
            onDownloadStatusChanged, onDownloadCompleted, cancellationToken);

        return data is null ? (string.Empty, downloadStatus) : (Encoding.UTF8.GetString(data), downloadStatus);
    }

    private static async Task<T?> DownloadAsync<T>(Func<CancellationToken, Task<T>> task,
        AutoRetriesPolicy? autoRetries,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(task);

        DownloadStatus _downloadStatus = new();

        autoRetries ??= new AutoRetriesPolicy
        {
            MaxRequestAutoRetries = 2,
            AutoRetriesDelay = TimeSpan.FromSeconds(value: 2)
        };

        var exceptions = new List<Exception>();

        T? result = default;

        do
        {
            --autoRetries.MaxRequestAutoRetries;

            try
            {
                result = await task(cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                _downloadStatus.Status =
                    cancellationToken.IsCancellationRequested ? TaskStatus.Canceled : TaskStatus.Faulted;

                exceptions.Add(ex.Demystify());
            }
            catch (HttpRequestException ex)
            {
                _downloadStatus.Status = TaskStatus.Faulted;
                exceptions.Add(ex.Demystify());
            }
            catch (Exception ex) when (ex.IsNetworkError())
            {
                _downloadStatus.Status = TaskStatus.Faulted;
                exceptions.Add(ex.Demystify());
            }

            if (exceptions.Count != 0)
            {
                // Wait a bit and try again later
                await Task.Delay(autoRetries.AutoRetriesDelay, cancellationToken);
            }
        }
        while (autoRetries.MaxRequestAutoRetries > 0 && _downloadStatus.Status != TaskStatus.RanToCompletion &&
               !cancellationToken.IsCancellationRequested);

        var uniqueExceptions = exceptions.Distinct(new ExceptionEqualityComparer()).ToList();

        if (uniqueExceptions.Count != 0 && _downloadStatus.Status != TaskStatus.RanToCompletion)
        {
            if (uniqueExceptions.Count() == 1)
            {
                throw uniqueExceptions[index: 0];
            }

            throw new AggregateException(message: "Could not process the request.", uniqueExceptions);
        }

        return result;
    }

    private static async Task<DownloadStatus> DoDownloadFileAsync(this HttpClient client,
        string url,
        string outputFilePath,
        Action<DownloadStatus>? onDownloadStatusChanged,
        Action<DownloadStatus>? onDownloadCompleted,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(client);

        DownloadStatus downloadStatus = new()
        {
            StartTime = DateTimeOffset.UtcNow,
            BytesTransferred = 0
        };

        var fileMode = FileMode.CreateNew;
        var fileInfo = new FileInfo(outputFilePath);

        if (fileInfo.Exists)
        {
            downloadStatus.BytesTransferred = fileInfo.Length;
            fileMode = FileMode.Append;
        }

        if (downloadStatus.BytesTransferred > 0)
        {
            client.DefaultRequestHeaders.Range = new RangeHeaderValue(downloadStatus.BytesTransferred, to: null);
        }

        using var response = await ReadResponseHeadersAsync(client, url, downloadStatus, cancellationToken);

        if (downloadStatus.BytesTransferred > 0 && downloadStatus.RemoteFileSize == downloadStatus.BytesTransferred)
        {
            ReportDownloadCompleted(downloadStatus, onDownloadCompleted, cancellationToken);

            return downloadStatus;
        }

        await using var inputStream = await response.Content.ReadAsStreamAsync(cancellationToken);

        await using var fileStream = outputFilePath.CreateAsyncFileStream(fileMode, FileAccess.Write);

        if (response.Headers.AcceptRanges is null && fileStream.Length > 0)
        {
            // Resume is not supported. Starting over.
            fileStream.SetLength(value: 0);
            await fileStream.FlushAsync(cancellationToken);
            downloadStatus.BytesTransferred = 0;
        }

        await ReadInputStreamAsync(inputStream, fileStream, downloadStatus, onDownloadStatusChanged, cancellationToken);

        ReportDownloadCompleted(downloadStatus, onDownloadCompleted, cancellationToken);

        return downloadStatus;
    }

    private static async Task ReadInputStreamAsync(Stream inputStream,
        Stream outputStream,
        DownloadStatus downloadStatus,
        Action<DownloadStatus>? onDownloadStatusChanged,
        CancellationToken cancellationToken)
    {
        var buffer = new byte[MaxBufferSize];
        int read;
        var readCount = 0L;

        while ((read = await inputStream.ReadAsync(buffer.AsMemory(start: 0, buffer.Length), cancellationToken)) > 0 &&
               !cancellationToken.IsCancellationRequested)
        {
            downloadStatus.BytesTransferred += read;
            downloadStatus.Status = TaskStatus.Running;
            readCount++;

            if (readCount % 100 == 0)
            {
                UpdateDownloadStatus(downloadStatus, onDownloadStatusChanged);
            }

            await outputStream.WriteAsync(buffer.AsMemory(start: 0, read), cancellationToken);
        }
    }

    private static async Task<HttpResponseMessage> ReadResponseHeadersAsync(HttpClient client,
        string url,
        DownloadStatus downloadStatus,
        CancellationToken cancellationToken)
    {
        client.DefaultRequestHeaders.ExpectContinue = false;
        client.DefaultRequestHeaders.Add(name: "Keep-Alive", value: "false");
        client.DefaultRequestHeaders.Add(name: "User-Agent", typeof(DownloaderService).Namespace);
        var uri = new Uri(url);
        client.DefaultRequestHeaders.Referrer = uri;

        var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await response.EnsureSuccessStatusCodeAsync(cancellationToken);

        downloadStatus.RemoteFileSize = response.Content.Headers?.ContentRange?.Length ??
                                        response.Content?.Headers?.ContentLength ?? 0;

        downloadStatus.RemoteFileName = response.Content?.Headers?.ContentDisposition?.FileName ?? string.Empty;

        return response;
    }

    private static async Task<(byte[] Data, DownloadStatus DownloadStatus)> DoDownloadDataAsync(this HttpClient client,
        string url,
        Action<DownloadStatus>? onDownloadCompleted,
        Action<DownloadStatus>? onDownloadStatusChanged,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(client);

        DownloadStatus downloadStatus = new()
        {
            StartTime = DateTimeOffset.UtcNow,
            BytesTransferred = 0
        };

        using var response = await ReadResponseHeadersAsync(client, url, downloadStatus, cancellationToken);

        await using var inputStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var outputStream = new MemoryStream();

        await ReadInputStreamAsync(inputStream, outputStream, downloadStatus, onDownloadStatusChanged,
            cancellationToken);

        await outputStream.FlushAsync(cancellationToken);

        ReportDownloadCompleted(downloadStatus, onDownloadCompleted, cancellationToken);

        return (outputStream.ToArray(), downloadStatus);
    }

    private static void ReportDownloadCompleted(DownloadStatus downloadStatus,
        Action<DownloadStatus>? onDownloadCompleted,
        CancellationToken cancellationToken)
    {
        downloadStatus.ElapsedDownloadTime = DateTimeOffset.UtcNow - downloadStatus.StartTime;

        downloadStatus.Status =
            cancellationToken.IsCancellationRequested ? TaskStatus.Canceled : TaskStatus.RanToCompletion;

        onDownloadCompleted?.Invoke(downloadStatus);
    }

    private static void UpdateDownloadStatus(DownloadStatus downloadStatus,
        Action<DownloadStatus>? onDownloadStatusChanged)
    {
        if (downloadStatus.RemoteFileSize <= 0 || downloadStatus.BytesTransferred <= 0)
        {
            return;
        }

        downloadStatus.PercentComplete =
            Math.Round((decimal)downloadStatus.BytesTransferred * 100 / downloadStatus.RemoteFileSize, decimals: 2);

        var elapsedTime = DateTimeOffset.UtcNow - downloadStatus.StartTime;
        downloadStatus.BytesPerSecond = downloadStatus.BytesTransferred / elapsedTime.TotalSeconds;

        var rawEta = downloadStatus.RemoteFileSize / downloadStatus.BytesPerSecond;
        downloadStatus.EstimatedCompletionTime = TimeSpan.FromSeconds(rawEta);

        onDownloadStatusChanged?.Invoke(downloadStatus);
    }
}
