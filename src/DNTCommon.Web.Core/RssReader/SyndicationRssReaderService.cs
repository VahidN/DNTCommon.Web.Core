namespace DNTCommon.Web.Core;

/// <summary>
///     Represents an RssReader service, in Atom 1.0 and in RSS 2.0.
/// </summary>
public sealed class SyndicationRssReaderService(IHttpClientFactory httpClientFactory) : IRssReaderService
{
    /// <summary>
    ///     Loads a syndication feed from the specified url.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<FeedChannel<FeedItem>> ReadRssAsync(string url, CancellationToken ct = default)
    {
        using var client = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);

        return await client.ReadRssAsync(url, ct);
    }
}
