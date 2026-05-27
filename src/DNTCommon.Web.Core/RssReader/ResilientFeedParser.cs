using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DNTCommon.Web.Core;

public static partial class ResilientFeedParser
{
    private static readonly string[] ElementsIndicators = ["/feed/entry", "/rss/channel/item"];
    private static readonly string[] ChannelsIndicators = ["/feed/", "/rss/channel/"];

    private static readonly string[] PublishDateIndicators =
    [
        "date", "issued", "published", "dc:date", "created", "pubdate", "lastbuilddate"
    ];

    private static readonly string[] LastUpdatedTimeIndicators = ["updated", "modified"];

    /// <summary>
    ///     Loads a syndication feed from the specified url, using HtmlAgilityPack
    /// </summary>
    public static async Task<FeedChannel<FeedItem>?> ReadRssResilientlyAsync(this HttpClient httpClient,
        [StringSyntax(syntax: "Uri")] string url,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        var rawXmlResult = await httpClient.SafeFetchAsync(url, cancellationToken: cancellationToken);

        if (rawXmlResult.Kind != FetchResultKind.Success)
        {
            throw new InvalidOperationException($"{url} -> {rawXmlResult.StatusCode} -> {rawXmlResult.Reason}");
        }

        return rawXmlResult.TextContent.ReadRssResiliently();
    }

    /// <summary>
    ///     Loads a syndication feed, using HtmlAgilityPack
    /// </summary>
    public static FeedChannel<FeedItem>? ReadRssResiliently(this string? xmlContent)
    {
        if (xmlContent.IsEmpty())
        {
            return null;
        }

        var doc = new HtmlDocument
        {
            OptionCheckSyntax = true,
            OptionFixNestedTags = true,
            OptionAutoCloseOnEnd = true,
            OptionDefaultStreamEncoding = Encoding.UTF8,
            OptionTreatCDataBlockAsComment = true
        };

        doc.LoadHtml(xmlContent);

        var feedChannel = new FeedChannel<FeedItem>();

        FeedItem? feedItem = null;

        foreach (var node in VisitChildren(doc.DocumentNode.ChildNodes))
        {
            var cleanedXPath = node.XPath.RemoveIndexesFromXPath();

            if (IsNewFeedItem(cleanedXPath))
            {
                feedItem = new FeedItem();
                feedChannel.RssItems.Add(feedItem);
            }
            else if (feedItem is not null && IsFeedItemElement(cleanedXPath))
            {
                SetFeedItemProperties(node, feedItem, cleanedXPath);
            }
            else if (IsChannelProperty(cleanedXPath))
            {
                SetFeedChannelProperties(feedChannel, node, cleanedXPath);
            }
        }

        feedChannel.NormalizeResults();

        return feedChannel;
    }

    private static void NormalizeResults(this FeedChannel<FeedItem> feedChannel)
    {
        feedChannel.RssItems ??= [];

        foreach (var item in feedChannel.RssItems)
        {
            if (item.PublishDate is null && item.LastUpdatedTime is null && feedChannel.LastUpdatedTime is not null)
            {
                item.PublishDate = feedChannel.LastUpdatedTime;
                item.LastUpdatedTime = feedChannel.LastUpdatedTime;
            }

            if (item.PublishDate is null && item.LastUpdatedTime is not null)
            {
                item.PublishDate = item.LastUpdatedTime;
            }

            if (item.PublishDate is not null && item.LastUpdatedTime is null)
            {
                item.LastUpdatedTime = item.PublishDate;
            }

            if (item.Content is null && item.Summary is not null)
            {
                item.Content = item.Summary;
            }

            if (item.Content is not null && item.Summary is null)
            {
                item.Summary = item.Content;
            }

            item.Categories ??= [];
        }
    }

    private static bool IsChannelProperty(string cleanedXPath)
        => ChannelsIndicators.Any(value => cleanedXPath.StartsWith(value, StringComparison.OrdinalIgnoreCase));

    private static bool IsFeedItemElement(string cleanedXPath)
        => ElementsIndicators.Any(value => cleanedXPath.StartsWith(value, StringComparison.OrdinalIgnoreCase));

    private static bool IsNewFeedItem(string cleanedXPath)
        => ElementsIndicators.Any(value => cleanedXPath.Equals(value, StringComparison.OrdinalIgnoreCase));

    private static void SetFeedItemProperties(HtmlNode node, FeedItem feedItem, string cleanedXPath)
    {
        var xPathElement = ElementsIndicators.Aggregate(cleanedXPath,
                (current, item) => current.Replace(item, newValue: "", StringComparison.OrdinalIgnoreCase))
            .TrimStart(trimChar: '/');

        var nodeText = GetNodeText(node);

        if (string.IsNullOrWhiteSpace(feedItem.Url) && xPathElement.IsOneOfItems("link"))
        {
            var possibleUrl = nodeText;

            if (possibleUrl.IsValidUrl())
            {
                feedItem.Url = possibleUrl;
            }
            else
            {
                var href = node.Attributes.FirstOrDefault(attr
                        => attr.Name.Equals(value: "href", StringComparison.OrdinalIgnoreCase))
                    ?.Value.CleanXmlValue();

                if (href.IsValidUrl())
                {
                    feedItem.Url = href;
                }
            }
        }

        if (string.IsNullOrWhiteSpace(nodeText))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(feedItem.Title) && xPathElement.IsOneOfItems("title"))
        {
            feedItem.Title = nodeText;
        }

        if (string.IsNullOrWhiteSpace(feedItem.AuthorName) &&
            xPathElement.IsOneOfItems("dc:creator", "author/name", "author/email", "creator"))
        {
            feedItem.AuthorName = nodeText;
        }

        if (string.IsNullOrWhiteSpace(feedItem.Summary) &&
            xPathElement.IsOneOfItems("description", "summary", "desc", "desclong"))
        {
            feedItem.Summary = nodeText;
        }

        if (string.IsNullOrWhiteSpace(feedItem.Content) && xPathElement.IsOneOfItems("content", "content:encoded"))
        {
            feedItem.Content = nodeText;
        }

        if (!string.IsNullOrWhiteSpace(nodeText) && xPathElement.IsOneOfItems("category"))
        {
            feedItem.Categories.Add(nodeText);
        }

        if (!feedItem.PublishDate.HasValue && xPathElement.IsOneOfItems(PublishDateIndicators))
        {
            feedItem.PublishDate = nodeText.TryParseFeedDate();
        }

        if (!feedItem.LastUpdatedTime.HasValue && xPathElement.IsOneOfItems(LastUpdatedTimeIndicators))
        {
            feedItem.LastUpdatedTime = nodeText.TryParseFeedDate();
        }
    }

    private static void SetFeedChannelProperties(FeedChannel<FeedItem> result, HtmlNode child, string cleanedXPath)
    {
        var xPathElement = ChannelsIndicators.Aggregate(cleanedXPath,
            (current, item) => current.Replace(item, newValue: "", StringComparison.OrdinalIgnoreCase));

        var nodeText = GetNodeText(child);

        if (string.IsNullOrWhiteSpace(nodeText))
        {
            return;
        }

        if (!result.LastUpdatedTime.HasValue && xPathElement.IsOneOfItems(
                [..PublishDateIndicators, ..LastUpdatedTimeIndicators]))
        {
            result.LastUpdatedTime = nodeText.TryParseFeedDate();
        }

        if (string.IsNullOrWhiteSpace(result.FeedTitle) && xPathElement.IsOneOfItems("title"))
        {
            result.FeedTitle = nodeText;
        }

        if (string.IsNullOrWhiteSpace(result.FeedDescription) &&
            xPathElement.IsOneOfItems("dc:creator", "creator", "description", "summary", "content"))
        {
            result.FeedDescription = nodeText;
        }

        if (string.IsNullOrWhiteSpace(result.FeedCopyright) && xPathElement.IsOneOfItems("copyright", "rights"))
        {
            result.FeedCopyright = nodeText;
        }

        if (string.IsNullOrWhiteSpace(result.FeedImageContentPath) &&
            xPathElement.IsOneOfItems("imageurl", "image/url"))
        {
            result.FeedImageContentPath = nodeText;
        }

        if (string.IsNullOrWhiteSpace(result.FeedImageTitle) && xPathElement.IsOneOfItems("image/title"))
        {
            result.FeedImageTitle = nodeText;
        }

        if (string.IsNullOrWhiteSpace(result.CultureName) && xPathElement.IsOneOfItems("language"))
        {
            result.CultureName = nodeText;
        }

        if (result.TimeToLive is null && xPathElement.IsOneOfItems("ttl"))
        {
            result.TimeToLive = GetTtl(nodeText);
        }
    }

    public static bool IsOneOfItems(this string xPathElement, params string[] elements)
        => elements.Any(value => xPathElement.Equals(value, StringComparison.OrdinalIgnoreCase));

    public static string RemoveIndexesFromXPath(this string xpath)
        => XPathIndexesRegex().Replace(xpath, replacement: "");

    private static string? GetNodeText(HtmlNode? child)
    {
        if (child is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(child.InnerHtml))
        {
            return child.InnerHtml.CleanXmlValue();
        }

        if (!string.IsNullOrWhiteSpace(child.InnerText))
        {
            return child.InnerText.CleanXmlValue();
        }

        if (child.NextSibling is not null)
        {
            return child.NextSibling.InnerHtml.CleanXmlValue();
        }

        return null;
    }

    private static string? CleanXmlValue(this string? xmlValue)
    {
        xmlValue = xmlValue?.Trim();

        if (string.IsNullOrWhiteSpace(xmlValue))
        {
            return null;
        }

        var startCData = xmlValue.IndexOf(value: "<![CDATA[", StringComparison.OrdinalIgnoreCase);
        var endCData = xmlValue.IndexOf(value: "]]>", StringComparison.OrdinalIgnoreCase);

        if (startCData >= 0 && endCData > startCData)
        {
            xmlValue = xmlValue.Substring(startCData + 9, endCData - startCData - 9);
        }

        return WebUtility.HtmlDecode(xmlValue).Trim();
    }

    private static IEnumerable<HtmlNode> VisitChildren(HtmlNodeCollection nodes)
    {
        foreach (var node in nodes)
        {
            yield return node;

            foreach (var childNode in VisitChildren(node.ChildNodes))
            {
                yield return childNode;
            }
        }
    }

    private static TimeSpan? GetTtl(string? ttlElement)
        => !string.IsNullOrWhiteSpace(ttlElement) && int.TryParse(ttlElement, NumberStyles.Number,
            CultureInfo.InvariantCulture, out var minutes)
            ? TimeSpan.FromMinutes(minutes)
            : null;

    [GeneratedRegex(pattern: @"\[\d+\]", RegexOptions.Compiled, matchTimeoutMilliseconds: 3000)]
    private static partial Regex XPathIndexesRegex();
}
