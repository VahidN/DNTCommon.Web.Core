using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using DNTPersianUtils.Core;
using DNTPersianUtils.Core.Normalizer;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// An ASP.NET Core RSS Feed Renderer.
    /// </summary>
    public class FeedResult : ActionResult
    {
        private const string Atom10Namespace = "https://www.w3.org/2005/Atom";
        private readonly List<SyndicationAttribute> _attributes = new List<SyndicationAttribute>
                {
                   new SyndicationAttribute("xmlns:atom", Atom10Namespace)
                };

        private readonly FeedChannel _feedChannel;
        private IHttpRequestInfoService _httpContextInfo;

        /// <summary>
        /// An ASP.NET Core RSS Feed Renderer.
        /// </summary>
        /// <param name="feedChannel">Channel's info</param>
        public FeedResult(FeedChannel feedChannel)
        {
            _feedChannel = feedChannel;
        }

        /// <summary>
        /// Executes the result operation of the action method asynchronously.
        /// </summary>
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _httpContextInfo = context.HttpContext.RequestServices.GetRequiredService<IHttpRequestInfoService>();
            await writeSyndicationFeedToResponseAsync(context);
        }

        private async Task writeSyndicationFeedToResponseAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            var mediaType = new MediaTypeHeaderValue("application/rss+xml")
            {
                CharSet = Encoding.UTF8.WebName
            };
            response.ContentType = mediaType.ToString();

            var xws = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, Async = true };
            using (var xmlWriter = XmlWriter.Create(response.Body, xws))
            {
                var formatter = new RssFormatter(_attributes, xmlWriter.Settings);
                var rssFeedWriter = await getRssFeedWriterAsync(xmlWriter);
                await writeSyndicationItemsAsync(formatter, rssFeedWriter);
                await xmlWriter.FlushAsync();
            }
        }

        private async Task writeSyndicationItemsAsync(RssFormatter formatter, RssFeedWriter rssFeedWriter)
        {
            foreach (var item in getSyndicationItems())
            {
                var content = new SyndicationContent(formatter.CreateContent(item));
                content.AddField(new SyndicationContent("atom:updated", Atom10Namespace, item.LastUpdated.ToString("r")));
                await rssFeedWriter.Write(content);
            }
        }

        private async Task<RssFeedWriter> getRssFeedWriterAsync(XmlWriter xmlWriter)
        {
            var rssFeedWriter = new RssFeedWriter(xmlWriter, _attributes);
            await addChannelIdentityAsync(rssFeedWriter);
            await addChannelLastUpdatedTimeAsync(rssFeedWriter);
            await addChannelImageAsync(rssFeedWriter);
            return rssFeedWriter;
        }

        private async Task addChannelLastUpdatedTimeAsync(RssFeedWriter rssFeedWriter)
        {
            if (_feedChannel.RssItems == null || !_feedChannel.RssItems.Any())
            {
                return;
            }

            await rssFeedWriter.WriteLastBuildDate(
                _feedChannel.RssItems.OrderByDescending(x => x.LastUpdatedTime).First().LastUpdatedTime);
        }

        private async Task addChannelIdentityAsync(RssFeedWriter rssFeedWriter)
        {
            await rssFeedWriter.WriteDescription(_feedChannel.FeedDescription.ApplyRle().RemoveHexadecimalSymbols());
            await rssFeedWriter.WriteCopyright(_feedChannel.FeedCopyright.ApplyRle().RemoveHexadecimalSymbols());
            await rssFeedWriter.WriteTitle(_feedChannel.FeedTitle.ApplyRle().RemoveHexadecimalSymbols());
            await rssFeedWriter.WriteLanguage(new CultureInfo(_feedChannel.CultureName));
            await rssFeedWriter.WriteRaw($"<atom:link href=\"{_httpContextInfo.GetRawUrl()}\" rel=\"self\" type=\"application/rss+xml\" />");
            await rssFeedWriter.Write(new SyndicationLink(_httpContextInfo.GetBaseUri(), relationshipType: RssElementNames.Link));
        }

        private async Task addChannelImageAsync(RssFeedWriter rssFeedWriter)
        {
            if (string.IsNullOrWhiteSpace(_feedChannel.FeedImageContentPath))
            {
                return;
            }

            var syndicationImage = new SyndicationImage(_httpContextInfo.AbsoluteContent(_feedChannel.FeedImageContentPath))
            {
                Title = _feedChannel.FeedImageTitle,
                Link = new SyndicationLink(_httpContextInfo.AbsoluteContent(_feedChannel.FeedImageContentPath))
            };
            await rssFeedWriter.Write(syndicationImage);
        }

        private IEnumerable<SyndicationItem> getSyndicationItems()
        {
            foreach (var item in _feedChannel.RssItems)
            {
                var uri = new Uri(QueryHelpers.AddQueryString(item.Url,
                                new Dictionary<string, string>
                                {
                                    { "utm_source", "feed" },
                                    { "utm_medium", "rss" },
                                    { "utm_campaign", "featured" },
                                    { "utm_updated", getUpdatedStamp(item) }
                                }));
                var syndicationItem = new SyndicationItem
                {
                    Title = item.Title.ApplyRle().RemoveHexadecimalSymbols(),
                    Id = uri.ToString(),
                    Description = item.Content.WrapInDirectionalDiv().RemoveHexadecimalSymbols(),
                    Published = item.PublishDate,
                    LastUpdated = item.LastUpdatedTime
                };
                syndicationItem.AddLink(new SyndicationLink(uri));
                syndicationItem.AddContributor(new SyndicationPerson(item.AuthorName, item.AuthorName));
                foreach (var category in item.Categories)
                {
                    syndicationItem.AddCategory(new SyndicationCategory(category));
                }
                yield return syndicationItem;
            }
        }

        private static string getUpdatedStamp(FeedItem item)
        {
            return item.LastUpdatedTime.ToShortPersianDateTimeString()
                                       .Replace("/", "-")
                                       .Replace(" ", "-")
                                       .Replace(":", "-");
        }
    }
}