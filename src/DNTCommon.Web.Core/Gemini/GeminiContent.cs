using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     The base structured datatype containing multipart content of a message.
///     A Content includes a <see cref="Role" /> field designating the producer of the Content and a <see cref="Parts" />
///     field containing multipart data that contains the content of the message turn.
/// </summary>
public class GeminiContent
{
    /// <summary>
    ///     The producer of the content. Must be either 'user' or 'model'.
    ///     Useful to set for multi-turn conversations, otherwise can be left blank or unset.
    /// </summary>
    [JsonPropertyName(name: "role")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Role { get; set; }

    /// <summary>
    ///     Ordered <see cref="GeminiPart" />s that constitute a single message. Parts may have different MIME types.
    /// </summary>
    [JsonPropertyName(name: "parts")]
    public List<GeminiPart>? Parts { get; set; }
}