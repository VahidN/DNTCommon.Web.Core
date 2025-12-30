using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     URI-based data with a specified media type.
/// </summary>
public sealed class GeminiFileData
{
    [JsonPropertyName(name: "mime_type")] public string MimeType { get; set; } = null!;

    [JsonPropertyName(name: "file_uri")] public Uri FileUri { get; set; } = null!;
}