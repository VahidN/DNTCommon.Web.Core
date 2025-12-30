using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     Supported modalities of the response.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<GeminiResponseModality>))]
public enum GeminiResponseModality
{
    /// <summary>
    ///     Default value.
    /// </summary>
    [JsonStringEnumMemberName(name: "MODALITY_UNSPECIFIED")]
    Unspecified,

    /// <summary>
    ///     Indicates the model should return text.
    /// </summary>
    [JsonStringEnumMemberName(name: "TEXT")]
    Text,

    /// <summary>
    ///     Indicates the model should return images.
    /// </summary>
    [JsonStringEnumMemberName(name: "IMAGE")]
    Image,

    /// <summary>
    ///     Indicates the model should return audio.
    /// </summary>
    [JsonStringEnumMemberName(name: "AUDIO")]
    Audio
}