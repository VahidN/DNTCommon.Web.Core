namespace DNTCommon.Web.Core;

public sealed record FetchResult(
    FetchResultKind Kind,
    Uri? FinalUri,
    HttpStatusCode? StatusCode,
    string? TextContent = null,
    byte[]? BinaryContent = null,
    string? ContentType = null,
    string? Reason = null);
