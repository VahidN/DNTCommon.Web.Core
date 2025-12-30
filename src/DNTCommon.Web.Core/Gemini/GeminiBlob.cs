using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     Raw data with a specified media type.
/// </summary>
public sealed class GeminiBlob
{
    /// <summary>
    ///     The IANA standard MIME type of the source data.
    /// </summary>
    [JsonPropertyName(name: "mime_type")]
    public string MimeType { get; set; } = null!;

    /// <summary>
    ///     Raw bytes for media formats. From a base64-encoded string.
    /// </summary>
    [JsonPropertyName(name: "data")]
    [JsonConverter(typeof(JsonStringBase64Converter))]
    public byte[] Data { get; set; } = null!;
}
