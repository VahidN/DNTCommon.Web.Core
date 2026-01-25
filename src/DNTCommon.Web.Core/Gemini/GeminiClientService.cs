using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace DNTCommon.Web.Core;

public class GeminiClientService(IHttpClientFactory httpClientFactory) : IGeminiClientService
{
    private const string BaseUrl = "https://generativelanguage.googleapis.com";

    public async Task<GeminiResponseResult<GeminiRemoteFile?>> FindLocalFileInGeminiFilesStoreAsync(string apiKey,
        string localFilePath,
        int? pageSize = 100,
        CancellationToken cancellationToken = default)
    {
        var queryString = GetQueryString(apiKey, pageSize, pageToken: null);
        var requestUri = $"{BaseUrl}/v1beta/files{queryString}";

        using var client = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var errorResponse = response.GetGeminiErrorResponse(responseBody);

        var responseResult = new GeminiResponseResult<GeminiRemoteFile?>
        {
            FullApiUri = new Uri(requestUri),
            IsSuccessfulResponse = errorResponse is null,
            ErrorResponse = errorResponse,
            ResponseBody = responseBody,
            Result = null
        };

        if (errorResponse is not null)
        {
            return responseResult;
        }

        var fileHash = localFilePath.GetContentsSHA256();

        var filesResponse =
            JsonSerializer.Deserialize<GeminiListFilesResponse>(responseBody, GeminiExtensions.DeserializeOptions);

        if (filesResponse?.Files is null)
        {
            return responseResult;
        }

        foreach (var file in filesResponse.Files)
        {
            if (file.Sha256Hash is null)
            {
                continue;
            }

            var sha256Hash = file.Sha256Hash.DecodeBase64();

            if (string.Equals(fileHash, sha256Hash, StringComparison.OrdinalIgnoreCase))
            {
                responseResult.Result = file;

                return responseResult;
            }
        }

        return responseResult;
    }

    public async Task<GeminiResponseResult<GeminiRemoteFile?>> FindRemoteFileInGeminiFilesStoreAsync(string apiKey,
        string remoteFileName,
        CancellationToken cancellationToken = default)
    {
        var requestUri =
            $"https://generativelanguage.googleapis.com/v1beta/{remoteFileName.GetRemoteGeminiFileId()}?key={apiKey}";

        using var client = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var errorResponse = response.GetGeminiErrorResponse(responseBody);

        var responseResult = new GeminiResponseResult<GeminiRemoteFile?>
        {
            FullApiUri = new Uri(requestUri),
            IsSuccessfulResponse = errorResponse is null,
            ErrorResponse = errorResponse,
            ResponseBody = responseBody,
            Result = null
        };

        if (errorResponse is not null)
        {
            return responseResult;
        }

        responseResult.Result =
            JsonSerializer.Deserialize<GeminiRemoteFile>(responseBody, GeminiExtensions.DeserializeOptions);

        return responseResult;
    }

    /// <summary>
    ///     Lists the metadata for <see cref="GeminiRemoteFile" />s owned by the requesting project.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="pageSize">
    ///     Maximum number of <see cref="GeminiRemoteFile" />s to return per page. If unspecified, defaults to 10.
    ///     Maximum <paramref name="pageSize" /> is 100.
    /// </param>
    /// <param name="pageToken">A page token from a previous <see cref="GetRemoteFilesListAsync" /> call.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A list of <see cref="GeminiRemoteFile" />s.</returns>
    /// <seealso href="https://ai.google.dev/api/files#method:-files.list">See Official API Documentation</seealso>
    public async Task<GeminiResponseResult<GeminiListFilesResponse?>> GetRemoteFilesListAsync(string apiKey,
        int? pageSize = null,
        string? pageToken = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = GetQueryString(apiKey, pageSize, pageToken);
        var requestUri = $"{BaseUrl}/v1beta/files{queryString}";

        using var client = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var errorResponse = response.GetGeminiErrorResponse(responseBody);

        var responseResult = new GeminiResponseResult<GeminiListFilesResponse?>
        {
            FullApiUri = new Uri(requestUri),
            IsSuccessfulResponse = errorResponse is null,
            ErrorResponse = errorResponse,
            ResponseBody = responseBody,
            Result = null
        };

        if (errorResponse is not null)
        {
            return responseResult;
        }

        responseResult.Result =
            JsonSerializer.Deserialize<GeminiListFilesResponse>(responseBody, GeminiExtensions.DeserializeOptions);

        return responseResult;
    }

    public async Task<GeminiResponseResult<GeminiFileUploadResponse?>> UploadFileToGeminiFilesStoreAsync(string apiKey,
        string localFilePath,
        string mimeType,
        CancellationToken cancellationToken = default)
    {
        var requestUri = $"{BaseUrl}/upload/v1beta/files?key={apiKey}&alt=json&uploadType=multipart";

        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

        using var multipartContent = new MultipartContent(subtype: "related");

        using var stringContent = new StringContent(JsonSerializer.Serialize(new
        {
            file = new
            {
                display_name = Path.GetFileNameWithoutExtension(localFilePath)
            }
        }), Encoding.UTF8, mediaType: "application/json");

        multipartContent.Add(stringContent);

        await using var fileStream = File.OpenRead(localFilePath);
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
        multipartContent.Add(streamContent);

        request.Content = multipartContent;

        using var client = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);
        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var errorResponse = response.GetGeminiErrorResponse(responseBody);

        var responseResult = new GeminiResponseResult<GeminiFileUploadResponse?>
        {
            FullApiUri = new Uri(requestUri),
            IsSuccessfulResponse = errorResponse is null,
            ErrorResponse = errorResponse,
            ResponseBody = responseBody,
            Result = null
        };

        if (errorResponse is not null)
        {
            return responseResult;
        }

        responseResult.Result =
            JsonSerializer.Deserialize<GeminiFileUploadResponse>(responseBody, GeminiExtensions.DeserializeOptions);

        return responseResult;
    }

    public async Task<GeminiResponseResult<GeminiGenerateContentResponse?>> RunGenerateContentPromptsAsync(
        GeminiClientOptions options,
        CancellationToken cancellationToken = default)
    {
        var request = options.CreateGeminiContentRequest();
        var requestUri = options.GetFullApiUri(BaseUrl);

        using var client = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);

        using var response =
            await client.PostAsJsonAsync(requestUri, request, GeminiExtensions.SerializeOptions, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var errorResponse = response.GetGeminiErrorResponse(responseBody);

        var responseResult = new GeminiResponseResult<GeminiGenerateContentResponse?>
        {
            FullApiUri = requestUri,
            IsSuccessfulResponse = errorResponse is null,
            ErrorResponse = errorResponse,
            ResponseBody = responseBody,
            Result = null
        };

        if (errorResponse is not null)
        {
            responseResult.Result = new GeminiGenerateContentResponse
            {
                ContentRequest = request,
                IsBlocked = false,
                IsFinished = false,
                ResponseContent = null
            };

            return responseResult;
        }

        var geminiResponse =
            JsonSerializer.Deserialize<GeminiResponseContent>(responseBody, GeminiExtensions.DeserializeOptions);

        if (geminiResponse?.PromptFeedback?.BlockReason is not null)
        {
            responseResult.Result = new GeminiGenerateContentResponse
            {
                ContentRequest = request,
                IsBlocked = true,
                IsFinished = false,
                ResponseContent = geminiResponse
            };

            return responseResult;
        }

        if (geminiResponse?.Candidates?.Any(responseCandidate
                => !string.Equals(responseCandidate.FinishReason, b: "stop", StringComparison.OrdinalIgnoreCase)) ==
            true)
        {
            responseResult.Result = new GeminiGenerateContentResponse
            {
                ContentRequest = request,
                IsBlocked = false,
                IsFinished = false,
                ResponseContent = geminiResponse
            };

            return responseResult;
        }

        responseResult.Result = new GeminiGenerateContentResponse
        {
            ContentRequest = request,
            IsBlocked = false,
            IsFinished = true,
            ResponseContent = geminiResponse
        };

        return responseResult;
    }

    private static string GetQueryString(string apiKey, int? pageSize, string? pageToken)
    {
        var queryParams = new List<string>
        {
            $"key={apiKey}"
        };

        if (pageSize.HasValue)
        {
            queryParams.Add(string.Create(CultureInfo.InvariantCulture, $"pageSize={pageSize.Value}"));
        }

        if (!string.IsNullOrEmpty(pageToken))
        {
            queryParams.Add($"pageToken={pageToken}");
        }

        var queryString = queryParams.Count > 0 ? "?" + string.Join(separator: '&', queryParams) : string.Empty;

        return queryString;
    }
}
