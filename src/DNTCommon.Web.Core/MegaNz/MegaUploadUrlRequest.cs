using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaUploadUrlRequest : MegaRequest
{
    public MegaUploadUrlRequest() => Action = "u";

    [JsonPropertyName(name: "s")] public long Size { get; set; }
}