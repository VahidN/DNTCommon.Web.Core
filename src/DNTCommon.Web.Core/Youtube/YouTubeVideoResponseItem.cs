using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public sealed class YouTubeVideoResponseItem
{
    [JsonPropertyName(name: "snippet")] public YouTubeVideoResponseSnippet Snippet { get; set; } = null!;
}
