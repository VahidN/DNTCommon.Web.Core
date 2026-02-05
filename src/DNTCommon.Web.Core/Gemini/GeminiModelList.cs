using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public class GeminiModelList
{
    [JsonPropertyName(name: "models")] public List<GeminiModelInfo> Models { get; set; } = [];
}
