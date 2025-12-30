using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     A response candidate generated from the model.
/// </summary>
public class GeminiResponseCandidate
{
    /// <summary>
    ///     Generated content returned from the model.
    /// </summary>
    [JsonPropertyName(name: "content")]
    public GeminiContent? Content { get; set; }

    /// <summary>
    ///     The reason why the model stopped generating tokens. If empty, the model has not stopped generating tokens.
    /// </summary>
    [JsonPropertyName(name: "finishReason")]
    public string? FinishReason { get; set; }

    /// <summary>
    ///     Index of the candidate in the list of response candidates.
    /// </summary>
    [JsonPropertyName(name: "index")]
    public int Index { get; set; }

    /// <summary>
    ///     List of ratings for the safety of a response candidate. There is at most one rating per category.
    /// </summary>
    [JsonPropertyName(name: "safetyRatings")]
    public List<GeminiSafetyRating>? SafetyRatings { get; set; }
}