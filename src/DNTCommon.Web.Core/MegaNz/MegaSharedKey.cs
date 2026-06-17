using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaSharedKey
{
    [JsonPropertyName(name: "h")] public string? Id { get; set; }

    [JsonPropertyName(name: "k")] public string? Key { get; set; }
}