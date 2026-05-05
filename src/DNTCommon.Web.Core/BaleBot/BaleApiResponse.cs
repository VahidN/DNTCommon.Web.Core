using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public class BaleApiResponse
{
    [JsonPropertyName(name: "ok")] public bool Ok { get; set; }
}
