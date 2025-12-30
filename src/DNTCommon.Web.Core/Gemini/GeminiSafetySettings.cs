using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public class GeminiSafetySettings
{
    [JsonPropertyName(name: "category")] public string? Category { get; set; }

    [JsonPropertyName(name: "threshold")] public string? Threshold { get; set; }
}