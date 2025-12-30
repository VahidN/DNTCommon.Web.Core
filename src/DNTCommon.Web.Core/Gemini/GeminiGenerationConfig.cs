using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     Configuration options for model generation and outputs. Not all parameters are configurable for every model.
/// </summary>
public class GeminiGenerationConfig
{
    /// <summary>
    ///     Controls the randomness of the output. Values can range from <c>[0.0, 2.0]</c>.
    /// </summary>
    [JsonPropertyName(name: "temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? Temperature { get; set; }

    /// <summary>
    ///     The maximum number of tokens to consider when sampling.
    /// </summary>
    [JsonPropertyName(name: "topK")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TopK { get; set; }

    /// <summary>
    ///     The maximum cumulative probability of tokens to consider when sampling.
    /// </summary>
    [JsonPropertyName(name: "topP")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? TopP { get; set; }

    /// <summary>
    ///     The maximum number of tokens to include in a response candidate.
    /// </summary>
    [JsonPropertyName(name: "maxOutputTokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxOutputTokens { get; set; }

    /// <summary>
    ///     Number of generated responses to return.
    /// </summary>
    [JsonPropertyName(name: "candidateCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CandidateCount { get; set; }

    /// <summary>
    ///     The requested modalities of the response. Represents the set of modalities that the model can return, and
    ///     should be expected in the response. This is an exact match to the modalities of the response. A model may have
    ///     multiple combinations of supported modalities. If the requested modalities do not match any of the supported
    ///     combinations, an error will be returned. An empty list is equivalent to requesting only text.
    /// </summary>
    [JsonPropertyName(name: "responseModalities")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ICollection<GeminiResponseModality>? ResponseModalities { get; set; }

    /// <summary>
    ///     The set of character sequences (up to 5) that will stop output generation. If specified, the API will stop at
    ///     the first appearance of a stop_sequence. The stop sequence will not be included as part of the response.
    /// </summary>
    [JsonPropertyName(name: "stopSequences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? StopSequences { get; set; }
}