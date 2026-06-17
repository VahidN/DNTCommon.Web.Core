using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

internal sealed class MegaLoginRequest : MegaRequest
{
    public MegaLoginRequest() => Action = "us";

    [JsonPropertyName(name: "user")] public string? User { get; set; }

    [JsonPropertyName(name: "uh")] public string? PasswordHash { get; set; }
}