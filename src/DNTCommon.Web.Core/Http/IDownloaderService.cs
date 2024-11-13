namespace DNTCommon.Web.Core;

/// <summary>
///     Downloader Service
/// </summary>
public interface IDownloaderService
{
    /// <summary>
    ///     Gives the current download operation's status.
    /// </summary>
    Action<DownloadStatus>? OnDownloadStatusChanged { set; get; }

    /// <summary>
    ///     It fires when the download operation is completed.
    /// </summary>
    Action<DownloadStatus>? OnDownloadCompleted { set; get; }

    /// <summary>
    ///     Downloads a file from a given url and then stores it as a local file.
    /// </summary>
    Task<DownloadStatus?> DownloadFileAsync(string url, string outputFilePath, AutoRetriesPolicy? autoRetries = null);

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a byte array.
    /// </summary>
    Task<(byte[] Data, DownloadStatus DownloadStatus)> DownloadDataAsync(string url,
        AutoRetriesPolicy? autoRetries = null);

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a text.
    /// </summary>
    Task<(string Data, DownloadStatus DownloadStatus)> DownloadPageAsync(string url,
        AutoRetriesPolicy? autoRetries = null);

    /// <summary>
    ///     Cancels the download operation.
    /// </summary>
    void CancelDownload();
}