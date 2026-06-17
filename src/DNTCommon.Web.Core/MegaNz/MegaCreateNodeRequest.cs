using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaCreateNodeRequest : MegaRequest
{
    public MegaCreateNodeRequest() => Action = "p";

    [JsonPropertyName(name: "t")] public string? ParentId { get; set; }

    [JsonPropertyName(name: "n")] public MegaCreateNodeData[]? Nodes { get; set; }
}