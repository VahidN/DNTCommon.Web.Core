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
/// <remarks>
///     An ASP.NET Core RSS Feed Renderer.
/// </remarks>
/// <param name="feedChannel">Channel's info</param>
public class FeedResult<TFeedItem>(FeedChannel<TFeedItem> feedChannel) : ActionResult
    where TFeedItem : FeedItem
{
    /// <summary>
    ///     Executes the result operation of the action method asynchronously.
    /// </summary>
    public override Task ExecuteResultAsync(ActionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var httpContextInfo = context.HttpContext.RequestServices.GetRequiredService<IHttpRequestInfoService>();

        return httpContextInfo == null
            ? throw new InvalidOperationException(message: "httpContextInfo is null.")
            : WriteSyndicationFeedToResponseAsync(context, httpContextInfo);
    }

    private Task WriteSyndicationFeedToResponseAsync(ActionContext context, IHttpRequestInfoService httpContextInfo)
    {
        var response = context.HttpContext.Response;

        response.ContentType = new MediaTypeHeaderValue(mediaType: "application/atom+xml")
        {
            CharSet = Encoding.UTF8.WebName
        }.ToString();

        var data = GetFeedData(httpContextInfo);

        return response.Body.WriteAsync(data.AsMemory(start: 0, data.Length)).AsTask();
    }

    private byte[] GetFeedData(IHttpRequestInfoService httpContextInfo)
    {
        using var memoryStream = new MemoryStream();

        using (var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings
               {
                   Indent = true,
                   Encoding = Encoding.UTF8
               }))
        {
            var feedFormatter = new Atom10FeedFormatter(GetSyndicationFeed(httpContextInfo));
            feedFormatter.WriteTo(xmlWriter);
        }

        return memoryStream.ToArray();
    }

    private SyndicationFeed GetSyndicationFeed(IHttpRequestInfoService httpContextInfo)
    {
        var rawUri = httpContextInfo.GetRawUri();
        var baseUri = httpContextInfo.GetBaseUri();

        var feed = new SyndicationFeed
        {
            Id = rawUri?.ToString(),
            Title = new TextSyndicationContent(feedChannel.FeedTitle.ApplyRle()
                .RemoveHexadecimalSymbols()
                .SanitizeXmlString()),
            Description = new TextSyndicationContent(feedChannel.FeedDescription.ApplyRle()
                .RemoveHexadecimalSymbols()
                .SanitizeXmlString()),
            Items = GetSyndicationItems(baseUri?.ToString() ?? "/"),
            Language = feedChannel.CultureName,
            Copyright = new TextSyndicationContent(feedChannel.FeedCopyright.ApplyRle()
                .RemoveHexadecimalSymbols()
                .SanitizeXmlString()),
            TimeToLive = feedChannel.TimeToLive,
            LastUpdatedTime = feedChannel.RssItems?.MaxBy(x => x.LastUpdatedTime)?.LastUpdatedTime ??
                              DateTimeOffset.UtcNow
        };

        feed.Links.Add(SyndicationLink.CreateAlternateLink(rawUri));

        if (!string.IsNullOrWhiteSpace(feedChannel.FeedImageContentPath))
        {
            feed.Links.Add(new SyndicationLink(httpContextInfo.AbsoluteContent(feedChannel.FeedImageContentPath)));
        }

        XNamespace atom = "http://www.w3.org/2005/Atom";

        feed.ElementExtensions.Add(new XElement(atom + "link", new XAttribute(name: "href", rawUri?.ToString() ?? ""),
            new XAttribute(name: "rel", value: "self"), new XAttribute(name: "type", value: "application/rss+xml")));

        return feed;
    }

    private IEnumerable<SyndicationItem> GetSyndicationItems(string baseUri)
    {
        if (feedChannel.RssItems is null)
        {
            yield break;
        }

        foreach (var item in feedChannel.RssItems)
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
                    Uri = baseUri
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