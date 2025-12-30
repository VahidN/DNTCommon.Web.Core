using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     A set of the feedback metadata the prompt specified in <see cref="GeminiResponseContent" />.
/// </summary>
public class GeminiResponsePromptFeedback
{
    /// <summary>
    ///     If set, the prompt was blocked and no candidates are returned. Rephrase the prompt.
    /// </summary>
    [JsonPropertyName(name: "blockReason")]
    public string? BlockReason { get; set; }

    /// <summary>
    ///     Ratings for safety of the prompt. There is at most one rating per category.
    /// </summary>
    [JsonPropertyName(name: "safetyRatings")]
    public List<GeminiSafetyRating>? SafetyRatings { get; set; }
}