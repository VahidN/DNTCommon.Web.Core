namespace DNTCommon.Web.Core;

/// <summary>
///     Represents an RssReader service, in Atom 1.0 and in RSS 2.0.
/// </summary>
public sealed class SyndicationRssReaderService : IRssReaderService
{
    private readonly HttpClient _client;

    /// <summary>
    ///     Represents an RssReader service, in Atom 1.0 and in RSS 2.0.
    /// </summary>
    public SyndicationRssReaderService(BaseHttpClient baseHttpClient)
    {
        var httpClient = baseHttpClient ?? throw new ArgumentNullException(nameof(baseHttpClient));
        _client = httpClient.HttpClient;
    }

    /// <summary>
    ///     Loads a syndication feed from the specified url.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public Task<FeedChannel<FeedItem>> ReadRssAsync(string url, CancellationToken ct = default)
        => _client.ReadRssAsync(url, ct);
}
