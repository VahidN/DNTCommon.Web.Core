using System.Text;
using System.Text.Json;

namespace DNTCommon.Web.Core;

/// <summary>
///     HttpClient Extensions
/// </summary>
public static class HttpClientExtensions
{
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
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        configRequest?.Invoke(httpRequestMessage);
        var response = await httpClient.SendAsync(httpRequestMessage);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync();
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
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, mediaType: "application/json");

        configRequest?.Invoke(httpRequestMessage);
        var response = await httpClient.SendAsync(httpRequestMessage);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync();
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
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, path);

        httpRequestMessage.Content = new FormUrlEncodedContent(formKeyValues);

        configRequest?.Invoke(httpRequestMessage);
        var response = await httpClient.SendAsync(httpRequestMessage);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync();
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
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        configRequest?.Invoke(request);
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync();
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(responseStream);

        return await streamReader.ReadToEndAsync();
    }

    /// <summary>
    ///     Downloads a URL as string.
    /// </summary>
    public static string DownloadPage(this HttpClient httpClient,
        string path,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        configRequest?.Invoke(request);
        var response = httpClient.Send(request, HttpCompletionOption.ResponseHeadersRead);

        if (ensureSuccess)
        {
            response.EnsureSuccessStatusCode();
        }

        using var responseStream = response.Content.ReadAsStream();
        using var streamReader = new StreamReader(responseStream);

        return streamReader.ReadToEnd();
    }

    /// <summary>
    ///     Downloads a URL as a binary file.
    /// </summary>
    public static async Task<byte[]> DownloadDataAsync(this HttpClient httpClient,
        string url,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        configRequest?.Invoke(request);
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync();
        }

        if (response.Content == null)
        {
            throw new InvalidOperationException($"`response.Content` of `{url}` is null!");
        }

        await using var inputStream = await response.Content.ReadAsStreamAsync();
        await using var memoryStream = new MemoryStream();
        await inputStream.CopyToAsync(memoryStream);

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
        var response = httpClient.Send(request, HttpCompletionOption.ResponseHeadersRead);

        if (ensureSuccess)
        {
            response.EnsureSuccessStatusCode();
        }

        if (response.Content == null)
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
        Action<HttpRequestMessage>? configRequest = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        configRequest?.Invoke(request);
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync();
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

        if (response.Content == null)
        {
            throw new InvalidOperationException($"`response.Content` of `{url}` is null!");
        }

        await SaveToFileAsync(outputFileNamePath, logger, response, remoteFileSize, tempFilePath);
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
        var response = httpClient.Send(request, HttpCompletionOption.ResponseHeadersRead);

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

        if (response.Content == null)
        {
            throw new InvalidOperationException($"`response.Content` of `{url}` is null!");
        }

        SaveToFile(outputFileNamePath, logger, response, remoteFileSize, tempFilePath);
    }

    private static async Task SaveToFileAsync(string outputFileNamePath,
        Action<string>? logger,
        HttpResponseMessage response,
        long remoteFileSize,
        string tempFilePath)
    {
        const int maxBufferSize = 0x10000;

        await using (var inputStream = await response.Content.ReadAsStreamAsync())
        {
            await using var fileStream = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, maxBufferSize, useAsync: true);

            var buffer = new byte[maxBufferSize];
            int read;
            var bytesTransferred = 0;
            var readCount = 0L;

            while ((read = await inputStream.ReadAsync(buffer.AsMemory(start: 0, buffer.Length))) > 0)
            {
                bytesTransferred += read;
                readCount++;

                if (remoteFileSize > 0 && bytesTransferred > 0)
                {
                    var percentComplete = Math.Round((decimal)bytesTransferred * 100 / remoteFileSize, decimals: 2);

                    if (readCount % 100 == 0)
                    {
                        logger?.Invoke(Invariant($"\rProgress: {percentComplete}%   "));
                    }
                }

                await fileStream.WriteAsync(buffer.AsMemory(start: 0, read));
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
        const int maxBufferSize = 0x10000;

        using (var inputStream = response.Content.ReadAsStream())
        {
            using var fileStream = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                maxBufferSize);

            var buffer = new byte[maxBufferSize];
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
                        logger?.Invoke(Invariant($"\rProgress: {percentComplete}%   "));
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
        bool throwOnException)
    {
        try
        {
            var responseMessage = await httpClient.GetAsync(siteUrl, ensureSuccess: false);

            return responseMessage.StatusCode;
        }
        catch (Exception ex)
        {
            if (throwOnException)
            {
                throw;
            }

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
            var responseMessage = httpClient.Get(siteUrl, ensureSuccess: false);

            return responseMessage.StatusCode;
        }
        catch (Exception ex)
        {
            if (throwOnException)
            {
                throw;
            }

            return (ex as HttpRequestException)?.StatusCode;
        }
    }
}