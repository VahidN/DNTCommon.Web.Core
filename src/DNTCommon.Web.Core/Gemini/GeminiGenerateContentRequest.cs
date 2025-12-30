using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public class GeminiGenerateContentRequest
{
    /// <summary>
    ///     Developer set system instruction(s). Currently, text only.
    /// </summary>
    [JsonPropertyName(name: "systemInstruction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeminiContent? SystemInstruction { get; init; }

    /// <summary>
    ///     The content of the current conversation with the model.
    ///     For single-turn queries, this is a single instance. For multi-turn queries like chat, this is a repeated field
    ///     that contains the conversation history and the latest request.
    /// </summary>
    [JsonPropertyName(name: "contents")]
    public List<GeminiContent>? Contents { get; set; }

    /// <summary>
    ///     Configuration options for model generation and outputs.
    /// </summary>
    [JsonPropertyName(name: "generationConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeminiGenerationConfig? GenerationConfig { get; set; }

    /// <summary>
    ///     A list of unique <see cref="SafetySettings" /> instances for blocking unsafe content.
    /// </summary>
    [JsonPropertyName(name: "safetySettings")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GeminiSafetySettings>? SafetySettings { get; set; }
}