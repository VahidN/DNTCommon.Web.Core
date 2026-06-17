using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaLoginResponse
{
    [JsonPropertyName(name: "csid")] public string? SessionId { get; set; }

    [JsonPropertyName(name: "privk")] public string? PrivateKey { get; set; }

    [JsonPropertyName(name: "k")] public string? MasterKey { get; set; }
}