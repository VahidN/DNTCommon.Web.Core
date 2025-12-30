using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     A datatype containing media that is part of a multipart Content message.
/// </summary>
public class GeminiPart
{
    /// <summary>
    ///     Inline text.
    /// </summary>
    [JsonPropertyName(name: "text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; set; }

    /// <summary>
    ///     Inline media bytes.
    /// </summary>
    [JsonPropertyName(name: "inlineData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeminiBlob? InlineData { get; set; }

    /// <summary>
    ///     URI based data.
    /// </summary>
    [JsonPropertyName(name: "fileData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeminiFileData? FileData { get; set; }
}