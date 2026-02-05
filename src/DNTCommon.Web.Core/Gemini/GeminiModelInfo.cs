using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public class GeminiModelInfo
{
    [JsonPropertyName(name: "name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName(name: "version")] public string Version { get; set; } = string.Empty;

    [JsonPropertyName(name: "displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName(name: "description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName(name: "inputTokenLimit")]
    public int InputTokenLimit { get; set; }

    [JsonPropertyName(name: "outputTokenLimit")]
    public int OutputTokenLimit { get; set; }

    [JsonPropertyName(name: "supportedGenerationMethods")]
    public List<string> SupportedGenerationMethods { get; set; } = [];

    [JsonPropertyName(name: "temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName(name: "topP")] public double? TopP { get; set; }

    [JsonPropertyName(name: "topK")] public int? TopK { get; set; }

    [JsonPropertyName(name: "maxTemperature")]
    public double? MaxTemperature { get; set; }

    [JsonPropertyName(name: "thinking")] public bool? Thinking { get; set; }
}
