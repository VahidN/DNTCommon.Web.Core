using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaExportLinkRequest : MegaRequest
{
    public MegaExportLinkRequest() => Action = "l";

    [JsonPropertyName(name: "n")] public string? NodeId { get; set; }

    // d=1 disables export link
    [JsonPropertyName(name: "d")] public int? Disable { get; set; }
}