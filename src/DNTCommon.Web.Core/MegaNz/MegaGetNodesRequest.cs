using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaGetNodesRequest : MegaRequest
{
    public MegaGetNodesRequest()
    {
        Action = "f";
        C = 1;
    }

    [JsonPropertyName(name: "c")] public int C { get; set; }
}