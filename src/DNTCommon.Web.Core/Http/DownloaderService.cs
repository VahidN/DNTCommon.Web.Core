﻿using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Lifetime of this class should be set to `Transient`.
///     Because setting HttpClient's `DefaultRequestHeaders` is not thread-safe and can't be shared across different
///     threads.
/// </summary>
public class DownloaderService : IDownloaderService, IDisposable
{
    private const int
        MaxBufferSize =
            0x10000; // 64K. The artificial constraint due to win32 api limitations. Increasing the buffer size beyond 64k will not help in any circumstance, as the underlying SMB protocol does not support buffer lengths beyond 64k.

    private readonly CancellationTokenSource _cancelSrc = new();
    private readonly HttpClient _client;
    private readonly DownloadStatus _downloadStatus = new();
    private bool _isDisposed;

    /// <summary>
    ///     Lifetime of this class should be set to `Transient`.
    ///     Because setting HttpClient's `DefaultRequestHeaders` is not thread-safe and can't be shared across different
    ///     threads.
    /// </summary>
    public DownloaderService(BaseHttpClient baseHttpClient)
    {
        var httpClient = baseHttpClient ?? throw new ArgumentNullException(nameof(baseHttpClient));
        _client = httpClient.HttpClient;
    }

    /// <summary>
    ///     Dispose all of the httpClients
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Gives the current download operation's status.
    /// </summary>
    public Action<DownloadStatus>? OnDownloadStatusChanged { set; get; }

    /// <summary>
    ///     It fires when the download operation is completed.
    /// </summary>
    public Action<DownloadStatus>? OnDownloadCompleted { set; get; }

    /// <summary>
    ///     Downloads a file from a given url and then stores it as a local file.
    /// </summary>
    public Task<DownloadStatus?> DownloadFileAsync(string url,
        string outputFilePath,
        AutoRetriesPolicy? autoRetries = null)
        => DownloadAsync(() => DoDownloadFileAsync(url, outputFilePath), autoRetries);

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a byte array.
    /// </summary>
    public Task<(byte[] Data, DownloadStatus DownloadStatus)> DownloadDataAsync(string url,
        AutoRetriesPolicy? autoRetries = null)
        => DownloadAsync(() => DoDownloadDataAsync(url), autoRetries);

    /// <summary>
    ///     Downloads a file from a given url and then returns it as a text.
    /// </summary>
    public async Task<(string Data, DownloadStatus DownloadStatus)> DownloadPageAsync(string url,
        AutoRetriesPolicy? autoRetries = null)
    {
        var result = await DownloadDataAsync(url, autoRetries);

        return result.Data == null
            ? (string.Empty, _downloadStatus)
            : (Encoding.UTF8.GetString(result.Data), _downloadStatus);
    }

    /// <summary>
    ///     Cancels the download operation.
    /// </summary>
    public void CancelDownload()
    {
        _downloadStatus.Status = TaskStatus.Canceled;
        _cancelSrc.Cancel();
        _client.CancelPendingRequests();
    }

    private async Task<T?> DownloadAsync<T>(Func<Task<T>> task, AutoRetriesPolicy? autoRetries)
    {
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
                result = await task();
            }
            catch (TaskCanceledException ex)
            {
                _downloadStatus.Status = _cancelSrc.IsCancellationRequested ? TaskStatus.Canceled : TaskStatus.Faulted;
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
                await Task.Delay(autoRetries.AutoRetriesDelay, _cancelSrc.Token);
            }
        }
        while (autoRetries.MaxRequestAutoRetries > 0 && _downloadStatus.Status != TaskStatus.RanToCompletion &&
               !_cancelSrc.IsCancellationRequested);

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

    /// <summary>
    ///     Dispose all of the httpClients
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            try
            {
                if (disposing)
                {
                    _cancelSrc?.Dispose();
                }
            }
            finally
            {
                _isDisposed = true;
            }
        }
    }

    private async Task<DownloadStatus> DoDownloadFileAsync(string url, string outputFilePath)
    {
        _downloadStatus.StartTime = DateTimeOffset.UtcNow;
        _downloadStatus.BytesTransferred = 0;

        var fileMode = FileMode.CreateNew;
        var fileInfo = new FileInfo(outputFilePath);

        if (fileInfo.Exists)
        {
            _downloadStatus.BytesTransferred = fileInfo.Length;
            fileMode = FileMode.Append;
        }

        if (_downloadStatus.BytesTransferred > 0)
        {
            _client.DefaultRequestHeaders.Range = new RangeHeaderValue(_downloadStatus.BytesTransferred, to: null);
        }

        var response = await ReadResponseHeadersAsync(url);

        if (_downloadStatus.BytesTransferred > 0 && _downloadStatus.RemoteFileSize == _downloadStatus.BytesTransferred)
        {
            ReportDownloadCompleted();

            return _downloadStatus;
        }

        await using var inputStream = await response.Content.ReadAsStreamAsync();

        await using var fileStream = new FileStream(outputFilePath, fileMode, FileAccess.Write, FileShare.None,
            MaxBufferSize,

            // you have to explicitly open the FileStream as asynchronous
            // or else you're just doing synchronous operations on a background thread.
            useAsync: true);

        if (response.Headers.AcceptRanges == null && fileStream.Length > 0)
        {
            // Resume is not supported. Starting over.
            fileStream.SetLength(value: 0);
            await fileStream.FlushAsync();
            _downloadStatus.BytesTransferred = 0;
        }

        await ReadInputStreamAsync(inputStream, fileStream);

        ReportDownloadCompleted();

        return _downloadStatus;
    }

    private async Task ReadInputStreamAsync(Stream inputStream, Stream outputStream)
    {
        var buffer = new byte[MaxBufferSize];
        int read;
        var readCount = 0L;

        while ((read = await inputStream.ReadAsync(buffer.AsMemory(start: 0, buffer.Length), _cancelSrc.Token)) > 0 &&
               !_cancelSrc.IsCancellationRequested)
        {
            _downloadStatus.BytesTransferred += read;
            _downloadStatus.Status = TaskStatus.Running;
            readCount++;

            if (readCount % 100 == 0)
            {
                UpdateDownloadStatus();
            }

            await outputStream.WriteAsync(buffer.AsMemory(start: 0, read), _cancelSrc.Token);
        }
    }

    private async Task<HttpResponseMessage> ReadResponseHeadersAsync(string url)
    {
        _client.DefaultRequestHeaders.ExpectContinue = false;
        _client.DefaultRequestHeaders.Add(name: "Keep-Alive", value: "false");
        _client.DefaultRequestHeaders.Add(name: "User-Agent", typeof(DownloaderService).Namespace);
        var uri = new Uri(url);
        _client.DefaultRequestHeaders.Referrer = uri;

        var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, _cancelSrc.Token);
        await response.EnsureSuccessStatusCodeAsync();

        _downloadStatus.RemoteFileSize = response.Content.Headers?.ContentRange?.Length ??
                                         response.Content?.Headers?.ContentLength ?? 0;

        _downloadStatus.RemoteFileName = response.Content?.Headers?.ContentDisposition?.FileName ?? string.Empty;

        return response;
    }

    private async Task<(byte[] Data, DownloadStatus DownloadStatus)> DoDownloadDataAsync(string url)
    {
        _downloadStatus.StartTime = DateTimeOffset.UtcNow;
        _downloadStatus.BytesTransferred = 0;

        var response = await ReadResponseHeadersAsync(url);

        await using var inputStream = await response.Content.ReadAsStreamAsync();
        using var outputStream = new MemoryStream();

        await ReadInputStreamAsync(inputStream, outputStream);
        await outputStream.FlushAsync();

        ReportDownloadCompleted();

        return (outputStream.ToArray(), _downloadStatus);
    }

    private void ReportDownloadCompleted()
    {
        _downloadStatus.ElapsedDownloadTime = DateTimeOffset.UtcNow - _downloadStatus.StartTime;
        _downloadStatus.Status = _cancelSrc.IsCancellationRequested ? TaskStatus.Canceled : TaskStatus.RanToCompletion;

        OnDownloadCompleted?.Invoke(_downloadStatus);
    }

    private void UpdateDownloadStatus()
    {
        if (_downloadStatus.RemoteFileSize <= 0 || _downloadStatus.BytesTransferred <= 0)
        {
            return;
        }

        _downloadStatus.PercentComplete =
            Math.Round((decimal)_downloadStatus.BytesTransferred * 100 / _downloadStatus.RemoteFileSize, decimals: 2);

        var elapsedTime = DateTimeOffset.UtcNow - _downloadStatus.StartTime;
        _downloadStatus.BytesPerSecond = _downloadStatus.BytesTransferred / elapsedTime.TotalSeconds;

        var rawEta = _downloadStatus.RemoteFileSize / _downloadStatus.BytesPerSecond;
        _downloadStatus.EstimatedCompletionTime = TimeSpan.FromSeconds(rawEta);

        OnDownloadStatusChanged?.Invoke(_downloadStatus);
    }
}