using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaDownloadUrlRequest : MegaRequest
{
    public MegaDownloadUrlRequest()
    {
        Action = "g";
        G = 1;
    }

    [JsonPropertyName(name: "g")] public int G { get; set; }

    [JsonPropertyName(name: "n")] public string? Id { get; set; }

    // For public links.
    [JsonPropertyName(name: "p")] public string? PublicHandle { get; set; }
}