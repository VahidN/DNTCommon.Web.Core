using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     Response from the model supporting multiple candidate responses.
/// </summary>
public class GeminiResponseContent
{
    /// <summary>
    ///     Candidate responses from the model.
    /// </summary>
    [JsonPropertyName(name: "candidates")]
    public List<GeminiResponseCandidate>? Candidates { get; set; }

    /// <summary>
    ///     Returns the prompt's feedback related to the content filters.
    /// </summary>
    [JsonPropertyName(name: "promptFeedback")]
    public GeminiResponsePromptFeedback? PromptFeedback { get; set; }

    /// <summary>
    ///     The model version used to generate the response.
    /// </summary>
    [JsonPropertyName(name: "modelVersion")]
    public string? ModelVersion { get; set; }
}