using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     Optional. Controls the maximum depth of the model's internal reasoning process before
///     it produces a response. If not specified, the default is HIGH. Recommended
///     for Gemini 3 or later models. Use with earlier models results in an error.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<GeminiThinkingConfigThinkingLevel>))]
public enum GeminiThinkingConfigThinkingLevel
{
    /// <summary>
    ///     Default value.
    /// </summary>
    [JsonStringEnumMemberName(name: "THINKING_LEVEL_UNSPECIFIED")]
    Unspecified,

    /// <summary>
    ///     Little to no thinking.
    /// </summary>
    [JsonStringEnumMemberName(name: "MINIMAL")]
    Minimal,

    /// <summary>
    ///     Low thinking level.
    /// </summary>
    [JsonStringEnumMemberName(name: "LOW")]
    Low,

    /// <summary>
    ///     Medium thinking level.
    /// </summary>
    [JsonStringEnumMemberName(name: "MEDIUM")]
    Medium,

    /// <summary>
    ///     High thinking level.
    /// </summary>
    [JsonStringEnumMemberName(name: "HIGH")]
    High
}
