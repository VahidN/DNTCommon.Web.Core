using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public class GeminiErrorDetails
{
    [JsonPropertyName(name: "code")] public HttpStatusCode StatusCode { get; set; }

    [JsonPropertyName(name: "message")] public string? Message { get; set; }

    [JsonPropertyName(name: "status")] public string? Status { get; set; }

    [JsonPropertyName(name: "details")] public JsonElement? Details { get; set; }
}
