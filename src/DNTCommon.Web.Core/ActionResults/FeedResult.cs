using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DNTPersianUtils.Core;
using DNTPersianUtils.Core.Normalizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     An ASP.NET Core RSS Feed Renderer.
/// </summary>
public class FeedResult : ActionResult
{
    private readonly FeedChannel _feedChannel;
    private IHttpRequestInfoService? _httpContextInfo;

    /// <summary>
    ///     An ASP.NET Core RSS Feed Renderer.
    /// </summary>
    /// <param name="feedChannel">Channel's info</param>
    public FeedResult(FeedChannel feedChannel) => _feedChannel = feedChannel;

    /// <summary>
    ///     Executes the result operation of the action method asynchronously.
    /// </summary>
    public override Task ExecuteResultAsync(ActionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        _httpContextInfo = context.HttpContext.RequestServices.GetRequiredService<IHttpRequestInfoService>();

        return WriteSyndicationFeedToResponseAsync(context);
    }

    private async Task WriteSyndicationFeedToResponseAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;

        var mediaType = new MediaTypeHeaderValue(mediaType: "application/atom+xml")
        {
            CharSet = Encoding.UTF8.WebName
        };

        response.ContentType = mediaType.ToString();

        var xws = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
            Async = true
        };

        await using var xmlWriter = XmlWriter.Create(response.Body, xws);
        var feedFormatter = new Atom10FeedFormatter(GetSyndicationFeed());
        feedFormatter.WriteTo(xmlWriter);
        await xmlWriter.FlushAsync();
    }

    private SyndicationFeed GetSyndicationFeed()
    {
        if (_httpContextInfo == null)
        {
            throw new InvalidOperationException(message: "_httpContextInfo is null.");
        }

        var feed = new SyndicationFeed
        {
            Id = _httpContextInfo.GetRawUri()?.ToString(),
            Title = new TextSyndicationContent(_feedChannel.FeedTitle.ApplyRle()
                .RemoveHexadecimalSymbols()
                .SanitizeXmlString()),
            Description = new TextSyndicationContent(_feedChannel.FeedDescription.ApplyRle()
                .RemoveHexadecimalSymbols()
                .SanitizeXmlString()),
            Items = GetSyndicationItems(),
            Language = _feedChannel.CultureName,
            Copyright = new TextSyndicationContent(_feedChannel.FeedCopyright.ApplyRle()
                .RemoveHexadecimalSymbols()
                .SanitizeXmlString()),
            TimeToLive = _feedChannel.TimeToLive,
            LastUpdatedTime = _feedChannel.RssItems.MaxBy(x => x.LastUpdatedTime)?.LastUpdatedTime ??
                              DateTimeOffset.UtcNow
        };

        feed.Links.Add(SyndicationLink.CreateAlternateLink(_httpContextInfo.GetRawUri()));

        if (!string.IsNullOrWhiteSpace(_feedChannel.FeedImageContentPath))
        {
            feed.Links.Add(new SyndicationLink(_httpContextInfo.AbsoluteContent(_feedChannel.FeedImageContentPath)));
        }

        XNamespace atom = "http://www.w3.org/2005/Atom";

        feed.ElementExtensions.Add(new XElement(atom + "link",
            new XAttribute(name: "href", _httpContextInfo.GetRawUri()?.ToString() ?? ""),
            new XAttribute(name: "rel", value: "self"), new XAttribute(name: "type", value: "application/rss+xml")));

        return feed;
    }

    private IEnumerable<SyndicationItem> GetSyndicationItems()
    {
        if (_httpContextInfo == null)
        {
            throw new InvalidOperationException(message: "_httpContextInfo is null.");
        }

        foreach (var item in _feedChannel.RssItems)
        {
            var uri = new Uri(QueryHelpers.AddQueryString(item.Url,
                new Dictionary<string, string?>(StringComparer.Ordinal)
                {
                    {
                        "utm_source", "feed"
                    },
                    {
                        "utm_medium", "rss"
                    },
                    {
                        "utm_campaign", "featured"
                    },
                    {
                        "utm_updated", GetUpdatedStamp(item)
                    }
                }));

            var syndicationItem = new SyndicationItem
            {
                Title =
                    new TextSyndicationContent(item.Title.ApplyRle().RemoveHexadecimalSymbols().SanitizeXmlString()),
                Id = uri.ToString(),
                Content = new TextSyndicationContent(
                    item.Content.WrapInDirectionalDiv(fontSize: "inherit")
                        .RemoveHexadecimalSymbols()
                        .SanitizeXmlString(), TextSyndicationContentKind.Html),
                Links =
                {
                    SyndicationLink.CreateAlternateLink(uri)
                },
                PublishDate = item.PublishDate,
                LastUpdatedTime = item.LastUpdatedTime
            };

            if (!string.IsNullOrWhiteSpace(item.AuthorName))
            {
                syndicationItem.Authors.Add(new SyndicationPerson
                {
                    Name = item.AuthorName,
                    Uri = _httpContextInfo.GetBaseUri()?.ToString()
                });
            }

            foreach (var category in item.Categories)
            {
                syndicationItem.Categories.Add(new SyndicationCategory(category));
            }

            yield return syndicationItem;
        }
    }

    private static string GetUpdatedStamp(FeedItem item)
        => item.LastUpdatedTime.ToShortPersianDateTimeString()
            .Replace(oldValue: "/", newValue: "-", StringComparison.Ordinal)
            .Replace(oldValue: " ", newValue: "-", StringComparison.Ordinal)
            .Replace(oldValue: ":", newValue: "-", StringComparison.Ordinal);
}