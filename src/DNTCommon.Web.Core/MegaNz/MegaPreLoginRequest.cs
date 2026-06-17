using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaPreLoginRequest : MegaRequest
{
    public MegaPreLoginRequest() => Action = "us0";

    [JsonPropertyName(name: "user")] public string? User { get; set; }
}