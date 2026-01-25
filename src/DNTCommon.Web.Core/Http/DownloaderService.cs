namespace DNTCommon.Web.Core;

/// <summary>
///     Downloader Service
/// </summary>
public class DownloaderService(IHttpClientFactory httpClientFactory) : IDownloaderService
{
    /// <summary>
    ///     Downloads a file from a given url and then stores it as a local file.
    /// </summary>
    public async Task<DownloadStatus?> DownloadFileAsync(string url,
        string outputFilePath,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);

        return await client.DownloadFileWithStatusReportAsync(url, outputFilePath, autoRetries, onDownloadStatusChanged,
            onDownloadCompleted, cancellationToken);
    }

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a byte array.
    /// </summary>
    public async Task<(byte[] Data, DownloadStatus DownloadStatus)> DownloadDataAsync(string url,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);

        return await client.DownloadDataWithStatusReportAsync(url, autoRetries, onDownloadStatusChanged,
            onDownloadCompleted, cancellationToken);
    }

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a text.
    /// </summary>
    public async Task<(string Data, DownloadStatus DownloadStatus)> DownloadPageAsync(string url,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);

        return await client.DownloadPageWithStatusReportAsync(url, autoRetries, onDownloadStatusChanged,
            onDownloadCompleted, cancellationToken);
    }
}
