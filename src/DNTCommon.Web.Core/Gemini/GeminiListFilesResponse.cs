using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     Response to list files.
/// </summary>
/// <seealso href="https://ai.google.dev/api/rest/v1beta/files/list">See Official API Documentation</seealso>
public class GeminiListFilesResponse
{
    /// <summary>
    ///     The list of <c>File</c>s.
    /// </summary>
    [JsonPropertyName(name: "files")]
    public List<GeminiRemoteFile>? Files { get; set; }

    /// <summary>
    ///     A token that can be sent as a <c>pageToken</c> into a subsequent <c>files.list</c> call.
    /// </summary>
    [JsonPropertyName(name: "nextPageToken")]
    public string? NextPageToken { get; set; }
}