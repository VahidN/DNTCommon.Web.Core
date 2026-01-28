using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public sealed class YouTubeVideoResponse
{
    [JsonPropertyName(name: "items")] public IList<YouTubeVideoResponseItem> Items { get; set; } = [];
}
