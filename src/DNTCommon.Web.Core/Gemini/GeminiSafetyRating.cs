using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public class GeminiSafetyRating
{
    /// <summary>
    ///     The category for this rating.
    /// </summary>
    [JsonPropertyName(name: "category")]
    public string? Category { get; set; }

    /// <summary>
    ///     The probability of harm for this content.
    /// </summary>
    [JsonPropertyName(name: "probability")]
    public string? Probability { get; set; }

    /// <summary>
    ///     Was this content blocked because of this rating?
    /// </summary>
    [JsonPropertyName(name: "blocked")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Blocked { get; set; }
}