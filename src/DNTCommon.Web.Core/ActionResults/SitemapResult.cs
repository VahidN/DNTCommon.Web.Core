using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.IO;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// SitemapItem's Change frequency
    /// </summary>
    public enum ChangeFrequency
    {
        /// <summary>
        /// Always
        /// </summary>
        Always,

        /// <summary>
        /// Hourly
        /// </summary>
        Hourly,

        /// <summary>
        /// Daily
        /// </summary>
        Daily,

        /// <summary>
        /// Weekly
        /// </summary>
        Weekly,

        /// <summary>
        /// Monthly
        /// </summary>
        Monthly,

        /// <summary>
        /// Yearly
        /// </summary>
        Yearly,

        /// <summary>
        /// Never
        /// </summary>
        Never
    }

    /// <summary>
    /// Represents a Sitemap entry
    /// </summary>
    public class SitemapItem
    {
        /// <summary>
        /// Item's absolute URL
        /// </summary>
        public string Url { set; get; }

        /// <summary>
        /// Item's Last Updated Time
        /// </summary>
        public DateTimeOffset LastUpdatedTime { set; get; }

        /// <summary>
        /// Change frequency. Its default value is `daily`.
        /// </summary>
        public ChangeFrequency ChangeFrequency { set; get; } = ChangeFrequency.Daily;

        /// <summary>
        /// Item's priority. Its default value is `0.5`.
        /// </summary>
        public decimal Priority { set; get; } = 0.5M;
    }

    /// <summary>
    /// Sitemap Result
    /// </summary>
    public class SitemapResult : ActionResult
    {
        readonly IList<SitemapItem> _allItems;

        /// <summary>
        /// Sitemap Result
        /// </summary>
        public SitemapResult(IList<SitemapItem> items)
        {
            _allItems = items;
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

            var httpContextInfo = context.HttpContext.RequestServices.GetRequiredService<IHttpRequestInfoService>();
            var response = context.HttpContext.Response;
            var mediaType = new MediaTypeHeaderValue("application/xml")
            {
                CharSet = Encoding.UTF8.WebName
            };
            response.ContentType = mediaType.ToString();

            var data = getSitemapData(httpContextInfo.GetBaseUrl());
            await response.Body.WriteAsync(data, 0, data.Length);
        }

        private byte[] getSitemapData(string baseUrl)
        {
            using (var memoryStream = new MemoryStream())
            {
                var xws = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
                using (var xmlWriter = XmlWriter.Create(memoryStream, xws))
                {
                    xmlWriter.WriteStartElement("urlset", "https://www.sitemaps.org/schemas/sitemap/0.9");
                    xmlWriter.WriteStartElement("url");
                    xmlWriter.WriteElementString("loc", baseUrl);
                    xmlWriter.WriteElementString("lastmod", DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    xmlWriter.WriteElementString("changefreq", "daily");
                    xmlWriter.WriteElementString("priority", "1.0");
                    xmlWriter.WriteEndElement();

                    foreach (var item in _allItems)
                    {
                        xmlWriter.WriteStartElement("url");
                        xmlWriter.WriteElementString("loc", item.Url);
                        xmlWriter.WriteElementString("lastmod", item.LastUpdatedTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                        xmlWriter.WriteElementString("changefreq", item.ChangeFrequency.ToString().ToLower());
                        xmlWriter.WriteElementString("priority", item.Priority.ToString(CultureInfo.InvariantCulture));
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }
                return memoryStream.ToArray();
            }
        }
    }
}