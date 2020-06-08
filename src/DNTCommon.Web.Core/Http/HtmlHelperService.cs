using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Html Helper Service Extensions
    /// </summary>
    public static class HtmlHelperServiceExtensions
    {
        /// <summary>
        /// Adds IHtmlHelperService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddHtmlHelperService(this IServiceCollection services)
        {
            services.AddScoped<IHtmlHelperService, HtmlHelperService>();
            return services;
        }
    }

    /// <summary>
    /// Html Helper Service
    /// </summary>
    public interface IHtmlHelperService
    {
        /// <summary>
        /// Returns the src list of img tags.
        /// </summary>
        IEnumerable<string> ExtractImagesLinks(string html);

        /// <summary>
        /// Returns the href list of anchor tags.
        /// </summary>
        IEnumerable<string> ExtractLinks(string html);

        /// <summary>
        /// Parses an HTML content and tries to convert its relative URLs to absolute urls based on the siteBaseUrl.
        /// </summary>
        string FixRelativeUrls(string html, string imageNotFoundPath, string siteBaseUrl);

        /// <summary>
        /// Parses an HTML content and tries to convert its relative URLs to absolute urls based on the siteBaseUrl.
        /// </summary>
        string FixRelativeUrls(string html, string imageNotFoundPath);

        /// <summary>
        /// Download the given uri and then extracts its title.
        /// </summary>
        Task<string> GetUrlTitleAsync(Uri uri);

        /// <summary>
        /// Extracts the given HTML page's title.
        /// </summary>
        string GetHtmlPageTitle(string html);

        /// <summary>
        /// Download the given uri and then extracts its title.
        /// </summary>
        Task<string> GetUrlTitleAsync(string url);
    }

    /// <summary>
    /// Html Helper Service
    /// </summary>
    public class HtmlHelperService : IHtmlHelperService
    {
        private readonly ILogger<HtmlHelperService> _logger;
        private readonly IDownloaderService _downloaderService;
        private readonly IHttpRequestInfoService _httpRequestInfoService;
        private readonly IHtmlReaderService _htmlReaderService;

        /// <summary>
        /// Html Helper Service
        /// </summary>
        public HtmlHelperService(
            ILogger<HtmlHelperService> logger,
            IDownloaderService downloaderService,
            IHttpRequestInfoService httpRequestInfoService,
            IHtmlReaderService htmlReaderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _downloaderService = downloaderService ?? throw new ArgumentNullException(nameof(downloaderService));
            _httpRequestInfoService = httpRequestInfoService ?? throw new ArgumentNullException(nameof(httpRequestInfoService));
            _htmlReaderService = htmlReaderService ?? throw new ArgumentNullException(nameof(htmlReaderService));
        }

        /// <summary>
        /// Returns the src list of img tags.
        /// </summary>
        public IEnumerable<string> ExtractImagesLinks(string html)
        {
            var doc = _htmlReaderService.CreateHtmlDocument(html);
            var nodes = doc.DocumentNode.SelectNodes("//img[@src]");
            if (nodes == null)
            {
                yield break;
            }

            foreach (HtmlNode image in nodes)
            {
                foreach (HtmlAttribute attribute in image.Attributes.Where(attr => attr.Name.Equals("src", StringComparison.OrdinalIgnoreCase)))
                {
                    yield return attribute.Value;
                }
            }
        }

        /// <summary>
        /// Returns HtmlAttribute's of the selected nodes.
        /// </summary>
        public IEnumerable<HtmlAttributeCollection> SelectNodes(string html, string xpath)
        {
            var doc = _htmlReaderService.CreateHtmlDocument(html);
            var nodes = doc.DocumentNode.SelectNodes(xpath);
            if (nodes == null)
            {
                yield break;
            }

            foreach (var item in nodes)
            {
                yield return item.Attributes;
            }
        }

        /// <summary>
        /// Returns the href list of anchor tags.
        /// </summary>
        public IEnumerable<string> ExtractLinks(string html)
        {
            var doc = _htmlReaderService.CreateHtmlDocument(html);
            var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
            if (nodes == null)
            {
                yield break;
            }

            foreach (var image in nodes)
            {
                foreach (var attribute in image.Attributes.Where(attr => attr.Name.Equals("href", StringComparison.OrdinalIgnoreCase)))
                {
                    yield return attribute.Value;
                }
            }
        }

        /// <summary>
        /// Parses an HTML content and tries to convert its relative URLs to absolute urls based on the siteBaseUrl.
        /// </summary>
        public string FixRelativeUrls(string html, string imageNotFoundPath, string siteBaseUrl)
        {
            var doc = _htmlReaderService.CreateHtmlDocument(html);
            var nodes = doc.DocumentNode.SelectNodes("//@background|//@lowsrc|//@src|//@href");
            if (nodes == null)
            {
                return doc.DocumentNode.OuterHtml;
            }

            foreach (var image in nodes)
            {
                foreach (var attribute in image.Attributes.Where(attr =>
                    attr.Name.Equals("background", StringComparison.OrdinalIgnoreCase) ||
                    attr.Name.Equals("lowsrc", StringComparison.OrdinalIgnoreCase) ||
                    attr.Name.Equals("src", StringComparison.OrdinalIgnoreCase) ||
                    attr.Name.Equals("href", StringComparison.OrdinalIgnoreCase)))
                {
                    var originalUrl = attribute.Value;

                    if (string.IsNullOrWhiteSpace(originalUrl))
                    {
                        attribute.Value = siteBaseUrl.CombineUrl(imageNotFoundPath);
                        _logger.LogWarning($"Changed URL: '' to '{attribute.Value}'.");
                        continue;
                    }

                    originalUrl = originalUrl.Trim();

                    if (originalUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        || originalUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                    {
                        if (attribute.Value != originalUrl)
                        {
                            _logger.LogWarning($"Changed URL: '{attribute.Value}' to '{originalUrl}'.");
                            attribute.Value = originalUrl;
                        }
                        continue;
                    }

                    var idx = originalUrl.IndexOf("data:image/", StringComparison.OrdinalIgnoreCase);
                    if (idx != -1)
                    {
                        var newImage = originalUrl.Substring(idx);
                        if (attribute.Value != newImage)
                        {
                            attribute.Value = newImage;
                            _logger.LogWarning($"Changed Image: '{originalUrl}' to '{attribute.Value}'.");
                        }
                        continue;
                    }

                    if (originalUrl.StartsWith("file:/", StringComparison.OrdinalIgnoreCase))
                    {
                        attribute.Value = siteBaseUrl.CombineUrl(imageNotFoundPath);
                        _logger.LogWarning($"Changed URL: '{originalUrl}' to '{attribute.Value}'.");
                        continue;
                    }

                    originalUrl = originalUrl.Replace("\\", "/").TrimStart('.').TrimStart('/').Trim();

                    var newUrl = $"http://{originalUrl}";
                    var (urlDomain, hasBestMatch) = newUrl.GetUrlDomain();
                    if (!string.IsNullOrWhiteSpace(urlDomain) && hasBestMatch)
                    {
                        attribute.Value = newUrl;
                        _logger.LogWarning($"Changed URL: '{originalUrl}' to '{newUrl}'.");
                        continue;
                    }

                    newUrl = siteBaseUrl.CombineUrl(originalUrl);
                    if (newUrl != attribute.Value)
                    {
                        attribute.Value = newUrl;
                        _logger.LogWarning($"Changed URL: '{originalUrl}' to '{newUrl}'.");
                    }
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// Parses an HTML content and tries to convert its relative URLs to absolute urls based on the siteBaseUrl.
        /// </summary>
        public string FixRelativeUrls(string html, string imageNotFoundPath)
        {
            return FixRelativeUrls(html, imageNotFoundPath, _httpRequestInfoService.GetBaseUrl());
        }

        /// <summary>
        /// Download the given uri and then extracts its title.
        /// </summary>
        public async Task<string> GetUrlTitleAsync(Uri uri)
        {
            var result = await _downloaderService.DownloadPageAsync(uri.ToString());
            return GetHtmlPageTitle(result.Data);
        }

        /// <summary>
        /// Extracts the given HTML page's title.
        /// </summary>
        public string GetHtmlPageTitle(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            var doc = _htmlReaderService.CreateHtmlDocument(html);
            var title = doc.DocumentNode.SelectSingleNode("//head/title");
            if (title == null)
            {
                return string.Empty;
            }

            var titleText = title.InnerText;
            if (string.IsNullOrWhiteSpace(titleText))
            {
                return string.Empty;
            }

            titleText = titleText.Trim()
                .Replace(Environment.NewLine, " ")
                .Replace("\t", " ")
                .Replace("\n", " ");
            return WebUtility.HtmlDecode(titleText.Trim());
        }

        /// <summary>
        /// Download the given uri and then extracts its title.
        /// </summary>
        public Task<string> GetUrlTitleAsync(string url)
        {
            return GetUrlTitleAsync(new Uri(url));
        }
    }
}