namespace DNTCommon.Web.Core;

/// <summary>
///     Downloader Service
/// </summary>
public interface IDownloaderService
{
    /// <summary>
    ///     Downloads a file from a given url and then stores it as a local file.
    /// </summary>
    Task<DownloadStatus?> DownloadFileAsync(string url,
        string outputFilePath,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a byte array.
    /// </summary>
    Task<(byte[] Data, DownloadStatus DownloadStatus)> DownloadDataAsync(string url,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a text.
    /// </summary>
    Task<(string Data, DownloadStatus DownloadStatus)> DownloadPageAsync(string url,
        AutoRetriesPolicy? autoRetries = null,
        Action<DownloadStatus>? onDownloadStatusChanged = null,
        Action<DownloadStatus>? onDownloadCompleted = null,
        CancellationToken cancellationToken = default);
}
