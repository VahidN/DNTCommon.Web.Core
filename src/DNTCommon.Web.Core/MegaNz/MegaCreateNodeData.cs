using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaCreateNodeData
{
    [JsonPropertyName(name: "h")] public string? CompletionHandle { get; set; }

    [JsonPropertyName(name: "t")] public int Type { get; set; }

    [JsonPropertyName(name: "a")] public string? Attributes { get; set; }

    [JsonPropertyName(name: "k")] public string? Key { get; set; }
}