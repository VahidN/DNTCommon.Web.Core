namespace DNTCommon.Web.Core;

public sealed record FetchResult(
    FetchResultKind Kind,
    Uri? FinalUri,
    HttpStatusCode? StatusCode,
    string? Content,
    string? Reason = null);
