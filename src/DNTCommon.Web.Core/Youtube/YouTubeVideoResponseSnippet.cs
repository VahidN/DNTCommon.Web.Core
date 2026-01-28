using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public sealed class YouTubeVideoResponseSnippet
{
    [JsonPropertyName(name: "title")] public string Title { get; set; } = string.Empty;

    [JsonPropertyName(name: "description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName(name: "channelTitle")]
    public string ChannelTitle { get; set; } = string.Empty;

    [JsonPropertyName(name: "tags")] public IList<string> Tags { get; set; } = [];
}
