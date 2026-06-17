using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaDeleteRequest : MegaRequest
{
    public MegaDeleteRequest() => Action = "d";

    [JsonPropertyName(name: "n")] public string? NodeId { get; set; }
}