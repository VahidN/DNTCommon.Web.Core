namespace DNTCommon.Web.Core;

public interface IGeminiClientService
{
    Task<GeminiResponseResult<GeminiRemoteFile?>> FindLocalFileInGeminiFilesStoreAsync(string apiKey,
        string localFilePath,
        int? pageSize = 100,
        CancellationToken cancellationToken = default);

    Task<GeminiResponseResult<GeminiRemoteFile?>> FindRemoteFileInGeminiFilesStoreAsync(string apiKey,
        string remoteFileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists the metadata for <see cref="GeminiRemoteFile" />s owned by the requesting project.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="pageSize">
    ///     Maximum number of <see cref="GeminiRemoteFile" />s to return per page. If unspecified, defaults to 10.
    ///     Maximum <paramref name="pageSize" /> is 100.
    /// </param>
    /// <param name="pageToken">A page token from a previous <see cref="GeminiClientService.GetRemoteFilesListAsync" /> call.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A list of <see cref="GeminiRemoteFile" />s.</returns>
    /// <seealso href="https://ai.google.dev/api/files#method:-files.list">See Official API Documentation</seealso>
    Task<GeminiResponseResult<GeminiListFilesResponse?>> GetRemoteFilesListAsync(string apiKey,
        int? pageSize = null,
        string? pageToken = null,
        CancellationToken cancellationToken = default);

    Task<GeminiResponseResult<GeminiFileUploadResponse?>> UploadFileToGeminiFilesStoreAsync(string apiKey,
        string localFilePath,
        string mimeType,
        CancellationToken cancellationToken = default);

    Task<GeminiResponseResult<GeminiGenerateContentResponse?>> RunGenerateContentPromptsAsync(
        GeminiClientOptions options,
        CancellationToken cancellationToken = default);
}
