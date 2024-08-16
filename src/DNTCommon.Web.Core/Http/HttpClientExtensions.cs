using System.Text;
using System.Text.Json;

namespace DNTCommon.Web.Core;

/// <summary>
///     HttpClient Extensions
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    ///     Allows manipulation of the request headers before it is sent, when you are using a signelton httpClient.
    /// </summary>
    public static async Task<HttpResponseMessage> GetAsync(this HttpClient httpClient,
        string uri,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null)
    {
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }

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
    ///     Allows manipulation of the request headers before it is sent, when you are using a signelton httpClient.
    /// </summary>
    public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient,
        string uri,
        T value,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null)
    {
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, mediaType: "application/json")
        };

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
    public static async Task<HttpResponseMessage> PostFormUrlEncodedContent(this HttpClient httpClient,
        IEnumerable<KeyValuePair<string?, string?>> formKeyValues,
        string path,
        bool ensureSuccess = true,
        Action<HttpRequestMessage>? configRequest = null)
    {
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = new FormUrlEncodedContent(formKeyValues)
        };

        configRequest?.Invoke(httpRequestMessage);
        var response = await httpClient.SendAsync(httpRequestMessage);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync();
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
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        configRequest?.Invoke(request);
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if (ensureSuccess)
        {
            await response.EnsureSuccessStatusCodeAsync();
        }

        using var responseStream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(responseStream);

        return await streamReader.ReadToEndAsync();
    }

    /// <summary>
    ///     Downloads a URL as a binary file.
    /// </summary>
    public static async Task DownloadFileAsync(this HttpClient httpClient,
        string url,
        string outputFileNamePath,
        bool ensureSuccess = true,
        Action<string>? logger = null,
        Action<HttpRequestMessage>? configRequest = null)
    {
        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }

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

        if (outputFileInfo.Exists)
        {
            var fileLength = outputFileInfo.Length;

            if (remoteFileSize > 0 && fileLength > 0 && fileLength == remoteFileSize)
            {
                logger?.Invoke($"`{outputFileNamePath}` file already exists.");

                return;
            }

            outputFileInfo.Delete();
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

        await saveToFile(outputFileNamePath, logger, response, remoteFileSize, tempFilePath);
    }

    private static async Task saveToFile(string outputFileNamePath,
        Action<string>? logger,
        HttpResponseMessage response,
        long remoteFileSize,
        string tempFilePath)
    {
        const int maxBufferSize = 0x10000;

        using (var inputStream = await response.Content.ReadAsStreamAsync())
        {
            using (var fileStream = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                       maxBufferSize, useAsync: true))
            {
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
}