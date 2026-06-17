using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     Direct download URL response.
/// </summary>
public sealed class MegaDownloadUrlResponse
{
    /// <summary>
    ///     Direct download URL.
    /// </summary>
    [JsonPropertyName(name: "g")]
    public string? Url { get; set; }

    /// <summary>
    ///     Size in bytes.
    /// </summary>
    [JsonPropertyName(name: "s")]
    public long Size { get; set; }
}