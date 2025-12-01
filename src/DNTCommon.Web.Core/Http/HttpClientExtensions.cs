using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     HttpClient Extensions
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    ///     Is the given remote url still available?
    /// </summary>
    public static async Task<bool> IsAvailableRemoteUrlAsync(this HttpClient httpClient,
        string url,
        Action<HttpRequestMessage>? configRequest = null,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        try
        {
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, url);
            configRequest?.Invoke(httpRequestMessage);
            using var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            await response.EnsureSuccessStatusCodeAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, message: "Error processing `{URL}`", url);

            if (ex is HttpRequestException hre)
            {
                if (hre.InnerException is SocketException { SocketErrorCode: SocketError.HostNotFound })
                {
                    return false;
                }

                if (hre.StatusCode.HasValue)
                {
                    var statusCode = (int)hre.StatusCode.Value;

                    switch (statusCode)
                    {
                        //Good requests
                        case >= 100 and < 400:
                            return true;

                        //Server Errors
                        case >= 500 and <= 510:
                            return false;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    ///     Allows manipulation of the request headers before it is sent, when you are using a singleton httpClient.
    /// </summary>
    public static HttpResponseMessage Get(this HttpClient httpClient,
        string uri,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        configRequest?.Invoke(httpRequestMessage);
        var response = httpClient.Send(httpRequestMessage);

        if (ensureSuccess)
        {
            response.EnsureSuccessStatusCode();
        }

        return response;
    }

    /// <summary>
    ///     Allows manipulation of the request headers before it is sent, when you are using a singleton httpClient.
    /// </summary>
    public static async Task<HttpResponseMessage> GetAsync(this HttpClient httpClient,
        string uri,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        configRequest?.Invoke(httpRequestMessage);
        var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync(cancellationToken);
        }

        return response;
    }

    /// <summary>
    ///     Allows manipulation of the request headers before it is sent, when you are using a singleton httpClient.
    /// </summary>
    public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient,
        string uri,
        T value,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, mediaType: "application/json");

        configRequest?.Invoke(httpRequestMessage);
        var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync(cancellationToken);
        }

        return response;
    }

    /// <summary>
    ///     Allows manipulation of the request headers before it is sent, when you are using a singleton httpClient.
    /// </summary>
    public static HttpResponseMessage PostAsJson<T>(this HttpClient httpClient,
        string uri,
        T value,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, mediaType: "application/json");

        configRequest?.Invoke(httpRequestMessage);
        var response = httpClient.Send(httpRequestMessage);

        if (ensureSuccess)
        {
            response.EnsureSuccessStatusCode();
        }

        return response;
    }

    /// <summary>
    ///     Posts a FormUrlEncodedContent to the server.
    /// </summary>
    public static async Task<HttpResponseMessage> PostFormUrlEncodedContentAsync(this HttpClient httpClient,
        IEnumerable<KeyValuePair<string?, string?>> formKeyValues,
        string path,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, path);

        httpRequestMessage.Content = new FormUrlEncodedContent(formKeyValues);

        configRequest?.Invoke(httpRequestMessage);
        var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync(cancellationToken);
        }

        return response;
    }

    /// <summary>
    ///     Posts a FormUrlEncodedContent to the server.
    /// </summary>
    public static HttpResponseMessage PostFormUrlEncodedContent(this HttpClient httpClient,
        IEnumerable<KeyValuePair<string?, string?>> formKeyValues,
        string path,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, path);

        httpRequestMessage.Content = new FormUrlEncodedContent(formKeyValues);

        configRequest?.Invoke(httpRequestMessage);
        var response = httpClient.Send(httpRequestMessage);

        if (ensureSuccess)
        {
            response.EnsureSuccessStatusCode();
        }

        return response;
    }

    /// <summary>
    ///     Downloads a URL as string.
    /// </summary>
    public static async Task<string> DownloadPageAsync(this HttpClient httpClient,
        string path,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null,
        Encoding? encoding = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        configRequest?.Invoke(request);

        using var response =
            await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync(cancellationToken);
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var streamReader = new StreamReader(responseStream, encoding ?? Encoding.UTF8);

#if !NET_6
        return await streamReader.ReadToEndAsync(cancellationToken);
#else
        return await streamReader.ReadToEndAsync();
#endif
    }

    /// <summary>
    ///     Downloads a URL as string.
    /// </summary>
    public static string DownloadPage(this HttpClient httpClient,
        string path,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null,
        Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        configRequest?.Invoke(request);
        using var response = httpClient.Send(request, HttpCompletionOption.ResponseHeadersRead);

        if (ensureSuccess)
        {
            response.EnsureSuccessStatusCode();
        }

        using var responseStream = response.Content.ReadAsStream();
        using var streamReader = new StreamReader(responseStream, encoding ?? Encoding.UTF8);

        return streamReader.ReadToEnd();
    }

    /// <summary>
    ///     Downloads a URL as a binary file.
    /// </summary>
    public static async Task<byte[]> DownloadDataAsync(this HttpClient httpClient,
        string url,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        configRequest?.Invoke(request);

        using var response =
            await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync(cancellationToken);
        }

        if (response.Content is null)
        {
            throw new InvalidOperationException($"`response.Content` of `{url}` is null!");
        }

        await using var inputStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var memoryStream = new MemoryStream();
        await inputStream.CopyToAsync(memoryStream, cancellationToken);

        return memoryStream.ToArray();
    }

    /// <summary>
    ///     Downloads a URL as a binary file.
    /// </summary>
    public static byte[] DownloadData(this HttpClient httpClient,
        string url,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        configRequest?.Invoke(request);
        using var response = httpClient.Send(request, HttpCompletionOption.ResponseHeadersRead);

        if (ensureSuccess)
        {
            response.EnsureSuccessStatusCode();
        }

        if (response.Content is null)
        {
            throw new InvalidOperationException($"`response.Content` of `{url}` is null!");
        }

        using var inputStream = response.Content.ReadAsStream();
        using var memoryStream = new MemoryStream();
        inputStream.CopyTo(memoryStream);

        return memoryStream.ToArray();
    }

    /// <summary>
    ///     Downloads a URL as a binary file.
    /// </summary>
    public static async Task DownloadFileAsync(this HttpClient httpClient,
        string url,
        string outputFileNamePath,
        bool allowOverwrite,
        bool ensureSuccess = true,
        Action<string>? logger = null,
        Action<HttpRequestMessage>? configRequest = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        configRequest?.Invoke(request);

        using var response =
            await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync(cancellationToken);
        }

        var remoteFileSize = response.Content.Headers?.ContentRange?.Length ??
                             response.Content?.Headers?.ContentLength ?? 0;

        logger?.Invoke($"File Size: {remoteFileSize.ToFormattedFileSize()}");

        var outputFileInfo = new FileInfo(outputFileNamePath);

        if (outputFileInfo.Exists && allowOverwrite)
        {
            var fileLength = outputFileInfo.Length;

            if (remoteFileSize > 0 && fileLength > 0 && fileLength == remoteFileSize)
            {
                logger?.Invoke($"`{outputFileNamePath}` file already exists.");

                return;
            }

            outputFileInfo.Delete();
        }
        else
        {
            var uploadsRootFolder = Path.GetDirectoryName(outputFileNamePath) ??
                                    throw new InvalidOperationException(message: "Save path is unknown.");

            outputFileNamePath = Path.GetFileName(outputFileNamePath).GetUniqueFilePath(uploadsRootFolder);
        }

        var tempFilePath = Path.ChangeExtension(outputFileNamePath, extension: ".tmp");

        if (File.Exists(tempFilePath))
        {
            File.Delete(tempFilePath);
        }

        if (response.Content is null)
        {
            throw new InvalidOperationException($"`response.Content` of `{url}` is null!");
        }

        await SaveToFileAsync(outputFileNamePath, logger, response, remoteFileSize, tempFilePath, cancellationToken);
    }

    /// <summary>
    ///     Downloads a URL as a binary file.
    /// </summary>
    public static void DownloadFile(this HttpClient httpClient,
        string url,
        string outputFileNamePath,
        bool allowOverwrite,
        bool ensureSuccess = true,
        Action<string>? logger = null,
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        configRequest?.Invoke(request);
        using var response = httpClient.Send(request, HttpCompletionOption.ResponseHeadersRead);

        if (ensureSuccess)
        {
            response.EnsureSuccessStatusCode();
        }

        var remoteFileSize = response.Content.Headers?.ContentRange?.Length ??
                             response.Content?.Headers?.ContentLength ?? 0;

        logger?.Invoke($"File Size: {remoteFileSize.ToFormattedFileSize()}");

        var outputFileInfo = new FileInfo(outputFileNamePath);

        if (outputFileInfo.Exists && allowOverwrite)
        {
            var fileLength = outputFileInfo.Length;

            if (remoteFileSize > 0 && fileLength > 0 && fileLength == remoteFileSize)
            {
                logger?.Invoke($"`{outputFileNamePath}` file already exists.");

                return;
            }

            outputFileInfo.Delete();
        }
        else
        {
            var uploadsRootFolder = Path.GetDirectoryName(outputFileNamePath) ??
                                    throw new InvalidOperationException(message: "Save path is unknown.");

            outputFileNamePath = Path.GetFileName(outputFileNamePath).GetUniqueFilePath(uploadsRootFolder);
        }

        var tempFilePath = Path.ChangeExtension(outputFileNamePath, extension: ".tmp");

        if (File.Exists(tempFilePath))
        {
            File.Delete(tempFilePath);
        }

        if (response.Content is null)
        {
            throw new InvalidOperationException($"`response.Content` of `{url}` is null!");
        }

        SaveToFile(outputFileNamePath, logger, response, remoteFileSize, tempFilePath);
    }

    private static async Task SaveToFileAsync(string outputFileNamePath,
        Action<string>? logger,
        HttpResponseMessage response,
        long remoteFileSize,
        string tempFilePath,
        CancellationToken cancellationToken)
    {
        const int MaxBufferSize = 0x10000;

        await using (var inputStream = await response.Content.ReadAsStreamAsync(cancellationToken))
        {
            await using var fileStream = tempFilePath.CreateAsyncFileStream(FileMode.CreateNew, FileAccess.Write);

            var buffer = new byte[MaxBufferSize];
            int read;
            var bytesTransferred = 0;
            var readCount = 0L;

            while ((read = await inputStream.ReadAsync(buffer.AsMemory(start: 0, buffer.Length), cancellationToken)) >
                   0)
            {
                bytesTransferred += read;
                readCount++;

                if (remoteFileSize > 0 && bytesTransferred > 0)
                {
                    var percentComplete = Math.Round((decimal)bytesTransferred * 100 / remoteFileSize, decimals: 2);

                    if (readCount % 100 == 0)
                    {
                        logger?.Invoke(
                            string.Create(CultureInfo.InvariantCulture, $"\rProgress: {percentComplete}%   "));
                    }
                }

                await fileStream.WriteAsync(buffer.AsMemory(start: 0, read), cancellationToken);
            }

            logger?.Invoke(obj: "\rProgress: 100%   \n");
        }

        File.Move(tempFilePath, outputFileNamePath);
    }

    private static void SaveToFile(string outputFileNamePath,
        Action<string>? logger,
        HttpResponseMessage response,
        long remoteFileSize,
        string tempFilePath)
    {
        const int MaxBufferSize = 0x10000;

        using (var inputStream = response.Content.ReadAsStream())
        {
            using var fileStream = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                MaxBufferSize);

            var buffer = new byte[MaxBufferSize];
            int read;
            var bytesTransferred = 0;
            var readCount = 0L;

            while ((read = inputStream.Read(buffer.AsSpan(start: 0, buffer.Length))) > 0)
            {
                bytesTransferred += read;
                readCount++;

                if (remoteFileSize > 0 && bytesTransferred > 0)
                {
                    var percentComplete = Math.Round((decimal)bytesTransferred * 100 / remoteFileSize, decimals: 2);

                    if (readCount % 100 == 0)
                    {
                        logger?.Invoke(
                            string.Create(CultureInfo.InvariantCulture, $"\rProgress: {percentComplete}%   "));
                    }
                }

                fileStream.Write(buffer.AsSpan(start: 0, read));
            }

            logger?.Invoke(obj: "\rProgress: 100%   \n");
        }

        File.Move(tempFilePath, outputFileNamePath);
    }

    /// <summary>
    ///     Gets the status code of the HTTP response.
    /// </summary>
    public static async Task<HttpStatusCode?> GetHttpStatusCodeAsync(this HttpClient httpClient,
        string siteUrl,
        bool throwOnException,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var responseMessage =
                await httpClient.GetAsync(siteUrl, ensureSuccess: false, cancellationToken: cancellationToken);

            return responseMessage.StatusCode;
        }
        catch (Exception ex) when (!throwOnException)
        {
            return (ex as HttpRequestException)?.StatusCode;
        }
    }

    /// <summary>
    ///     Gets the status code of the HTTP response.
    /// </summary>
    public static HttpStatusCode? GetHttpStatusCode(this HttpClient httpClient, string siteUrl, bool throwOnException)
    {
        try
        {
            using var responseMessage = httpClient.Get(siteUrl, ensureSuccess: false);

            return responseMessage.StatusCode;
        }
        catch (Exception ex) when (!throwOnException)
        {
            return (ex as HttpRequestException)?.StatusCode;
        }
    }
}
