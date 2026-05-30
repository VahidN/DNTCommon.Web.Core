using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     Config for thinking features.
/// </summary>
public sealed record GeminiThinkingConfiguration
{
    /// <summary>
    ///     Indicates whether to include thoughts in the response.
    ///     If true, thoughts are returned only when available.
    /// </summary>
    [JsonPropertyName(name: "includeThoughts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IncludeThoughts { get; init; }

    /// <summary>
    ///     The number of thoughts tokens that the model should generate.
    /// </summary>
    [JsonPropertyName(name: "thinkingBudget")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ThinkingBudget { get; init; }

    /// <summary>
    ///     Optional. Controls the maximum depth of the model's internal reasoning process before
    ///     it produces a response. If not specified, the default is HIGH. Recommended
    ///     for Gemini 3 or later models. Use with earlier models results in an error.
    /// </summary>
    [JsonPropertyName(name: "thinkingLevel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public GeminiThinkingConfigThinkingLevel? ThinkingLevel { get; init; }
}
