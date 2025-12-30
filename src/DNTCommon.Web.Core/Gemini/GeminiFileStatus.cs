using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     The <c>Status</c> type defines a logical error model that is suitable for different
///     programming environments, including REST APIs and RPC APIs. It is used by
///     <see href="https://github.com/grpc">gRPC</see>. Each <c>Status</c> message contains
///     three pieces of data: error code, error message, and error details.
///     You can find out more about this error model and how to work with it in the
///     <see href="https://cloud.google.com/apis/design/errors">API Design Guide</see>.
/// </summary>
/// <seealso href="https://ai.google.dev/api/files#status">See Official API Documentation</seealso>
public class GeminiFileStatus
{
    /// <summary>
    ///     The status code, which should be an enum value of <c>google.rpc.Code</c>.
    /// </summary>
    [JsonPropertyName(name: "code")]
    public int Code { get; set; }

    /// <summary>
    ///     A developer-facing error message, which should be in English. Any user-facing error
    ///     message should be localized and sent in the Details
    ///     field, or
    ///     localized by the client.
    /// </summary>
    [JsonPropertyName(name: "message")]
    public string? Message { get; set; }

    /// <summary>
    ///     A list of messages that carry the error details. There is a common set of
    ///     message types for APIs to use.
    ///     An object containing fields of an arbitrary type. An additional field <c>@type</c>
    ///     contains a URI identifying the type.
    ///     Example: <c>{ "id": 1234, "@type": "types.example.com/standard/id" }</c>.
    /// </summary>
    [JsonPropertyName(name: "details")]
    public List<Dictionary<string, JsonElement>>? Details { get; set; }
}
