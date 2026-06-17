using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaUploadUrlResponse
{
    [JsonPropertyName(name: "p")] public string? Url { get; set; }
}