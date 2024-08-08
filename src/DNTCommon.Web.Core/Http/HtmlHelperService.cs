using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Html Helper Service
/// </summary>
/// <remarks>
///     Html Helper Service
/// </remarks>
public class HtmlHelperService(
    ILogger<HtmlHelperService> logger,
    IDownloaderService downloaderService,
    IHttpRequestInfoService httpRequestInfoService,
    IHtmlReaderService htmlReaderService,
    IAntiXssService antiXssService) : IHtmlHelperService
{
    private static readonly TimeSpan oneMinute = TimeSpan.FromMinutes(value: 1);

    private static readonly Regex _htmlSpacesPattern =
        new(pattern: @"&nbsp;|&zwnj;|(\n)\s*", RegexOptions.Compiled, oneMinute);

    private readonly IDownloaderService _downloaderService =
        downloaderService ?? throw new ArgumentNullException(nameof(downloaderService));

    private readonly IHtmlReaderService _htmlReaderService =
        htmlReaderService ?? throw new ArgumentNullException(nameof(htmlReaderService));

    private readonly IHttpRequestInfoService _httpRequestInfoService =
        httpRequestInfoService ?? throw new ArgumentNullException(nameof(httpRequestInfoService));

    private readonly ILogger<HtmlHelperService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Returns the src list of img tags.
    /// </summary>
    public IEnumerable<string> ExtractImagesLinks(string html)
    {
        var doc = _htmlReaderService.CreateHtmlDocument(html);
        var nodes = doc.DocumentNode.SelectNodes(xpath: "//img[@src]");

        if (nodes == null)
        {
            yield break;
        }

        foreach (var image in nodes)
        {
            foreach (var attribute in image.Attributes.Where(attr
                         => attr.Name.Equals(value: "src", StringComparison.OrdinalIgnoreCase)))
            {
                yield return attribute.Value;
            }
        }
    }

    /// <summary>
    ///     Returns the href list of anchor tags.
    /// </summary>
    public IEnumerable<string> ExtractLinks(string html)
    {
        var doc = _htmlReaderService.CreateHtmlDocument(html);
        var nodes = doc.DocumentNode.SelectNodes(xpath: "//a[@href]");

        if (nodes == null)
        {
            yield break;
        }

        foreach (var image in nodes)
        {
            foreach (var attribute in image.Attributes.Where(attr
                         => attr.Name.Equals(value: "href", StringComparison.OrdinalIgnoreCase)))
            {
                yield return attribute.Value;
            }
        }
    }

    /// <summary>
    ///     Parses an HTML content and tries to convert its relative URLs to absolute urls based on the siteBaseUrl.
    /// </summary>
    public string FixRelativeUrls(string html, string imageNotFoundPath, string siteBaseUrl)
    {
        var doc = _htmlReaderService.CreateHtmlDocument(html);
        var nodes = doc.DocumentNode.SelectNodes(xpath: "//@background|//@lowsrc|//@src|//@href");

        if (nodes == null)
        {
            return doc.DocumentNode.OuterHtml;
        }

        foreach (var image in nodes)
        {
            foreach (var attribute in image.Attributes.Where(attr
                         => attr.Name.Equals(value: "background", StringComparison.OrdinalIgnoreCase) ||
                            attr.Name.Equals(value: "lowsrc", StringComparison.OrdinalIgnoreCase) ||
                            attr.Name.Equals(value: "src", StringComparison.OrdinalIgnoreCase) ||
                            attr.Name.Equals(value: "href", StringComparison.OrdinalIgnoreCase)))
            {
                var originalUrl = attribute.Value;

                if (string.IsNullOrWhiteSpace(originalUrl))
                {
                    attribute.Value = siteBaseUrl.CombineUrl(imageNotFoundPath);

                    _logger.LogWarning(message: "Changed URL: '' to '{AttributeValue}'.",
                        antiXssService.GetSanitizedHtml(attribute.Value));

                    continue;
                }

                originalUrl = originalUrl.Trim();

                if (originalUrl.StartsWith(value: "http", StringComparison.OrdinalIgnoreCase) ||
                    originalUrl.StartsWith(value: "https", StringComparison.OrdinalIgnoreCase))
                {
                    if (!attribute.Value.Equals(originalUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning(message: "Changed URL: '{AttributeValue}' to '{OriginalUrl}'.",
                            antiXssService.GetSanitizedHtml(attribute.Value),
                            antiXssService.GetSanitizedHtml(originalUrl));

                        attribute.Value = originalUrl;
                    }

                    continue;
                }

                var idx = originalUrl.IndexOf(value: "data:image/", StringComparison.OrdinalIgnoreCase);

                if (idx != -1)
                {
                    var newImage = originalUrl.Substring(idx);

                    if (!attribute.Value.Equals(newImage, StringComparison.OrdinalIgnoreCase))
                    {
                        attribute.Value = newImage;

                        _logger.LogWarning(message: "Changed Image: '{OriginalUrl}' to '{AttributeValue}'.",
                            antiXssService.GetSanitizedHtml(originalUrl),
                            antiXssService.GetSanitizedHtml(attribute.Value));
                    }

                    continue;
                }

                if (originalUrl.StartsWith(value: "file:/", StringComparison.OrdinalIgnoreCase))
                {
                    attribute.Value = siteBaseUrl.CombineUrl(imageNotFoundPath);

                    _logger.LogWarning(message: "Changed URL: '{OriginalUrl}' to '{AttributeValue}'.",
                        antiXssService.GetSanitizedHtml(originalUrl), antiXssService.GetSanitizedHtml(attribute.Value));

                    continue;
                }

                originalUrl = originalUrl.Replace(oldValue: "\\", newValue: "/", StringComparison.Ordinal)
                    .TrimStart(trimChar: '.')
                    .TrimStart(trimChar: '/')
                    .Trim();

                var newUrl = $"http://{originalUrl}";
                var (urlDomain, hasBestMatch) = newUrl.GetUrlDomain();

                if (!string.IsNullOrWhiteSpace(urlDomain) && hasBestMatch)
                {
                    attribute.Value = newUrl;

                    _logger.LogWarning(message: "Changed URL: '{OriginalUrl}' to '{NewUrl}'.",
                        antiXssService.GetSanitizedHtml(originalUrl), antiXssService.GetSanitizedHtml(newUrl));

                    continue;
                }

                newUrl = siteBaseUrl.CombineUrl(originalUrl);

                if (!newUrl.Equals(attribute.Value, StringComparison.OrdinalIgnoreCase))
                {
                    attribute.Value = newUrl;

                    _logger.LogWarning(message: "Changed URL: '{OriginalUrl}' to '{NewUrl}'.",
                        antiXssService.GetSanitizedHtml(originalUrl), antiXssService.GetSanitizedHtml(newUrl));
                }
            }
        }

        return doc.DocumentNode.OuterHtml;
    }

    /// <summary>
    ///     Parses an HTML content and tries to convert its relative URLs to absolute urls based on the siteBaseUrl.
    /// </summary>
    public string FixRelativeUrls(string html, string imageNotFoundPath)
    {
        var siteBaseUrl = _httpRequestInfoService.GetBaseUrl();

        if (string.IsNullOrWhiteSpace(siteBaseUrl))
        {
            throw new InvalidOperationException(message: "`siteBaseUrl` is null.");
        }

        return FixRelativeUrls(html, imageNotFoundPath, siteBaseUrl);
    }

    /// <summary>
    ///     Download the given uri and then extracts its title.
    /// </summary>
    public async Task<string> GetUrlTitleAsync(Uri uri)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        var result = await _downloaderService.DownloadPageAsync(uri.ToString());

        return GetHtmlPageTitle(result.Data);
    }

    /// <summary>
    ///     Download the given uri and then extracts its title.
    /// </summary>
    public Task<string> GetUrlTitleAsync(string url) => GetUrlTitleAsync(new Uri(url));

    /// <summary>
    ///     Extracts the given HTML page's title.
    /// </summary>
    public string GetHtmlPageTitle(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var doc = _htmlReaderService.CreateHtmlDocument(html);
        var title = doc.DocumentNode.SelectSingleNode(xpath: "//head/title");

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
            .Replace(Environment.NewLine, newValue: " ", StringComparison.Ordinal)
            .Replace(oldValue: "\t", newValue: " ", StringComparison.Ordinal)
            .Replace(oldValue: "\n", newValue: " ", StringComparison.Ordinal);

        return WebUtility.HtmlDecode(titleText.Trim());
    }

    /// <summary>
    ///     Removes all of the HTML tags.
    /// </summary>
    public string RemoveHtmlTags(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var doc = _htmlReaderService.CreateHtmlDocument(html);
        var innerText = doc.DocumentNode.InnerText;

        return string.IsNullOrWhiteSpace(innerText)
            ? string.Empty
            : _htmlSpacesPattern.Replace(innerText, replacement: " ").Trim();
    }

    /// <summary>
    ///     An enhanced version of HttpUtility.HtmlEncode method
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string? FullHtmlEncode(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        var chars = HttpUtility.HtmlEncode(text).ToCharArray();
        var result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

        foreach (var c in chars)
        {
            var value = Convert.ToInt32(c);

            if (value > 127)
            {
                result.Append(CultureInfo.InvariantCulture, $"&#{value};");
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    /// <summary>
    ///     Returns HtmlAttribute's of the selected nodes.
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
}