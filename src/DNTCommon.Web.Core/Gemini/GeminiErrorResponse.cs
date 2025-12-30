using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     An error response from the Gemini API.
/// </summary>
public class GeminiErrorResponse
{
    /// <summary>
    ///     The error details.
    /// </summary>
    [JsonPropertyName(name: "error")]
    public GeminiErrorDetails? Error { get; set; }
}