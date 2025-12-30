using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     A file uploaded to the API.
/// </summary>
/// <seealso href="https://ai.google.dev/api/files#File">See Official API Documentation</seealso>
public class GeminiRemoteFile
{
    /// <summary>
    ///     Immutable. Identifier. The <c>File</c> resource name. The ID (name excluding the "files/" prefix)
    ///     can contain up to 40 characters that are lowercase alphanumeric or dashes (-). The ID cannot start
    ///     or end with a dash. If the name is empty on create, a unique name will be generated.
    ///     Example: <c>files/123-456</c>
    /// </summary>
    [JsonPropertyName(name: "name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Optional. The human-readable display name for the <c>File</c>. The display name must be no more than
    ///     512 characters in length, including spaces. Example: "Welcome Image"
    /// </summary>
    [JsonPropertyName(name: "displayName")]
    public string? DisplayName { get; set; }

    /// <summary>
    ///     Output only. MIME type of the file.
    /// </summary>
    [JsonPropertyName(name: "mimeType")]
    public string? MimeType { get; set; }

    /// <summary>
    ///     Output only. Size of the file in bytes.
    /// </summary>
    [JsonPropertyName(name: "sizeBytes")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long? SizeBytes { get; set; }

    /// <summary>
    ///     Output only. The timestamp of when the <c>File</c> was created.
    ///     Uses RFC 3339, where generated output will always be Z-normalized and uses 0, 3, 6 or 9 fractional
    ///     digits. Offsets other than "Z" are also accepted.
    ///     Examples: <c>"2014-10-02T15:01:23Z"</c>, <c>"2014-10-02T15:01:23.045123456Z"</c> or
    ///     <c>"2014-10-02T15:01:23+05:30"</c>.
    /// </summary>
    [JsonPropertyName(name: "createTime")]
    public GeminiTimestamp? CreateTime { get; set; }

    /// <summary>
    ///     Output only. The timestamp of when the <c>File</c> was last updated.
    ///     Uses RFC 3339, where generated output will always be Z-normalized and uses 0, 3, 6 or 9 fractional
    ///     digits. Offsets other than "Z" are also accepted.
    ///     Examples: <c>"2014-10-02T15:01:23Z"</c>, <c>"2014-10-02T15:01:23.045123456Z"</c> or
    ///     <c>"2014-10-02T15:01:23+05:30"</c>.
    /// </summary>
    [JsonPropertyName(name: "updateTime")]
    public GeminiTimestamp? UpdateTime { get; set; }

    /// <summary>
    ///     Output only. The timestamp of when the <c>File</c> will be deleted. Only set if the <c>File</c> is
    ///     scheduled to expire.
    ///     Uses RFC 3339, where generated output will always be Z-normalized and uses 0, 3, 6 or 9 fractional
    ///     digits. Offsets other than "Z" are also accepted.
    ///     Examples: <c>"2014-10-02T15:01:23Z"</c>, <c>"2014-10-02T15:01:23.045123456Z"</c> or
    ///     <c>"2014-10-02T15:01:23+05:30"</c>.
    /// </summary>
    [JsonPropertyName(name: "expirationTime")]
    public GeminiTimestamp? ExpirationTime { get; set; }

    /// <summary>
    ///     Output only. SHA-256 hash of the uploaded bytes.
    ///     A base64-encoded string.
    /// </summary>
    [JsonPropertyName(name: "sha256Hash")]
    public string? Sha256Hash { get; set; }

    /// <summary>
    ///     Output only. The uri of the <c>File</c>.
    /// </summary>
    [JsonPropertyName(name: "uri")]
    public string? Uri { get; set; }

    /// <summary>
    ///     Output only. The download uri of the <c>File</c>.
    /// </summary>
    [JsonPropertyName(name: "downloadUri")]
    public string? DownloadUri { get; set; }

    /// <summary>
    ///     Output only. Processing state of the File.
    /// </summary>
    [JsonPropertyName(name: "state")]
    public GeminiFileState? State { get; set; }

    /// <summary>
    ///     Source of the File.
    /// </summary>
    [JsonPropertyName(name: "source")]
    public GeminiFileSource? Source { get; set; }

    /// <summary>
    ///     Output only. Error status if File processing failed.
    /// </summary>
    [JsonPropertyName(name: "error")]
    public GeminiFileStatus? Error { get; set; }

    /// <summary>
    ///     Output only. Metadata for a video.
    /// </summary>
    [JsonPropertyName(name: "videoMetadata")]
    public GeminiVideoMetadata? VideoMetadata { get; set; }
}