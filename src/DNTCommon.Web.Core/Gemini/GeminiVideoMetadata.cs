using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     Metadata for a video <c>File</c>.
/// </summary>
/// <seealso href="https://ai.google.dev/api/files#VideoMetadata">See Official API Documentation</seealso>
public class GeminiVideoMetadata
{
    /// <summary>
    ///     Duration of the video.
    ///     A duration in seconds with up to nine fractional digits, ending with '<c>s</c>'.
    ///     Example: <c>"3.5s"</c>.
    /// </summary>
    [JsonPropertyName(name: "videoDuration")]
    public GeminiDuration? VideoDuration { get; set; }
}