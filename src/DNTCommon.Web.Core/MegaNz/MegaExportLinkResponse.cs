using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaExportLinkResponse
{
    [JsonPropertyName(name: "ph")] public string? PublicHandle { get; set; }
}