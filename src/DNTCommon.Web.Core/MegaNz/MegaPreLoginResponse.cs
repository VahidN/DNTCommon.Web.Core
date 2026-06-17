using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaPreLoginResponse
{
    [JsonPropertyName(name: "v")] public int Version { get; set; }

    [JsonPropertyName(name: "s")] public string? Salt { get; set; }
}