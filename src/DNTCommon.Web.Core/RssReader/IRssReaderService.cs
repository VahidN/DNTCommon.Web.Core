namespace DNTCommon.Web.Core;

/// <summary>
///     Represents an RssReader service, in Atom 1.0 and in RSS 2.0.
/// </summary>
public interface IRssReaderService
{
    /// <summary>
    ///     Loads a syndication feed from the specified url.
    /// </summary>
    Task<FeedChannel<FeedItem>> ReadRssAsync(string url, CancellationToken ct = default);
}
