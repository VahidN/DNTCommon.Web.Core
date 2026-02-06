using System.ServiceModel.Syndication;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     IRssReaderService Extensions
/// </summary>
public static class RssReaderServiceExtensions
{
    /// <summary>
    ///     Adds IRssReaderService to IServiceCollection
    /// </summary>
    public static IServiceCollection AddRssReaderService(this IServiceCollection services)
    {
        services.AddScoped<IRssReaderService, SyndicationRssReaderService>();

        return services;
    }

    /// <summary>
    ///     Loads a syndication feed from the specified url.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<FeedChannel<FeedItem>> ReadRssAsync(this HttpClient httpClient,
        string url,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        var rawXmlResult = await httpClient.SafeFetchAsync(url, cancellationToken: cancellationToken);

        if (rawXmlResult.Kind != FetchResultKind.Success || rawXmlResult.TextContent.IsEmpty())
        {
            throw new InvalidOperationException($"{url} -> {rawXmlResult.StatusCode} -> {rawXmlResult.Reason}");
        }

        using var stringReader = new StringReader(rawXmlResult.TextContent);
        using var xmlReader = new RssXmlReader(stringReader);

        var feed = SyndicationFeed.Load(xmlReader) ?? throw new InvalidOperationException(message: "Invalid RSS feed");

        return new FeedChannel<FeedItem>
        {
            FeedTitle = feed.Title?.Text ?? string.Empty,
            FeedDescription = feed.Description?.Text ?? string.Empty,
            FeedCopyright = feed.Copyright?.Text ?? string.Empty,
            FeedImageContentPath = feed.ImageUrl?.ToString() ?? string.Empty,
            FeedImageTitle = feed.Title?.Text ?? string.Empty,
            CultureName = feed.Language ?? string.Empty,
            TimeToLive = GetTtl(feed),
            RssItems = feed.Items.Select(MapItem).ToList()
        };
    }

    private static FeedItem MapItem(SyndicationItem item)
        => new()
        {
            Title = item.Title?.Text ?? string.Empty,
            AuthorName = GetAuthorName(item),
            Content = GetContent(item),
            Categories = item.Categories.Select(c => c.Name).ToList(),
            Url = item.Links.FirstOrDefault(l => l.RelationshipType == "alternate")?.Uri.ToString() ??
                  item.Links.FirstOrDefault()?.Uri.ToString() ?? string.Empty,
            PublishDate = item.PublishDate != DateTimeOffset.MinValue ? item.PublishDate : item.LastUpdatedTime,
            LastUpdatedTime = item.LastUpdatedTime != DateTimeOffset.MinValue ? item.LastUpdatedTime : item.PublishDate
        };

    private static string GetAuthorName(SyndicationItem item)
    {
        var author = item.Authors.FirstOrDefault();

        return author is null ? string.Empty : author.Name ?? author.Email ?? string.Empty;
    }

    private static string GetContent(SyndicationItem item)
    {
        if (item.Content is TextSyndicationContent textContent)
        {
            return textContent.Text;
        }

        return item.Summary?.Text ?? string.Empty;
    }

    private static TimeSpan? GetTtl(SyndicationFeed feed)
    {
        var ttlElement = feed.ElementExtensions.ReadElementExtensions<string>(extensionName: "ttl", string.Empty)
            .FirstOrDefault();

        return int.TryParse(ttlElement, NumberStyles.Number, CultureInfo.InvariantCulture, out var minutes)
            ? TimeSpan.FromMinutes(minutes)
            : null;
    }
}
