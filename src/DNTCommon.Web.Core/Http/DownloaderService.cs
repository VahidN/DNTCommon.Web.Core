namespace DNTCommon.Web.Core;

/// <summary>
///     Downloader Service
/// </summary>
public class DownloaderService : IDownloaderService
{
    private readonly HttpClient _client;

    /// <summary>
    ///     Downloader Service
    /// </summary>
    public DownloaderService(BaseHttpClient baseHttpClient)
    {
        var httpClient = baseHttpClient ?? throw new ArgumentNullException(nameof(baseHttpClient));
        _client = httpClient.HttpClient;
    }

    /// <summary>
    ///     Downloads a file from a given url and then stores it as a local file.
    /// </summary>
    public Task<DownloadStatus?> DownloadFileAsync(string url,
        string outputFilePath,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default)
        => _client.DownloadFileWithStatusReportAsync(url, outputFilePath, autoRetries, onDownloadStatusChanged,
            onDownloadCompleted, cancellationToken);

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a byte array.
    /// </summary>
    public Task<(byte[] Data, DownloadStatus DownloadStatus)> DownloadDataAsync(string url,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default)
        => _client.DownloadDataWithStatusReportAsync(url, autoRetries, onDownloadStatusChanged, onDownloadCompleted,
            cancellationToken);

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a text.
    /// </summary>
    public Task<(string Data, DownloadStatus DownloadStatus)> DownloadPageAsync(string url,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default)
        => _client.DownloadPageWithStatusReportAsync(url, autoRetries, onDownloadStatusChanged, onDownloadCompleted,
            cancellationToken);
}
