using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal abstract class MegaRequest
{
    [JsonPropertyName(name: "a")] public string? Action { get; protected set; }

    // Request id echoed by server in some cases; required by some commands.
    [JsonPropertyName(name: "i")] public string? RequestId { get; set; }
}