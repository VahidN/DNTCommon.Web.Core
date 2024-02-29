using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Html Helper Service
/// </summary>
public class HtmlHelperService : IHtmlHelperService
{
    private static readonly TimeSpan oneMinute = TimeSpan.FromMinutes(1);

    private static readonly Regex _htmlSpacesPattern = new(@"&nbsp;|&zwnj;|(\n)\s*", RegexOptions.Compiled, oneMinute);

    private readonly IDownloaderService _downloaderService;
    private readonly IHtmlReaderService _htmlReaderService;
    private readonly IHttpRequestInfoService _httpRequestInfoService;

    private readonly ILogger<HtmlHelperService> _logger;

    /// <summary>
    ///     Html Helper Service
    /// </summary>
    public HtmlHelperService(ILogger<HtmlHelperService> logger,
        IDownloaderService downloaderService,
        IHttpRequestInfoService httpRequestInfoService,
        IHtmlReaderService htmlReaderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _downloaderService = downloaderService ?? throw new ArgumentNullException(nameof(downloaderService));

        _httpRequestInfoService =
            httpRequestInfoService ?? throw new ArgumentNullException(nameof(httpRequestInfoService));

        _htmlReaderService = htmlReaderService ?? throw new ArgumentNullException(nameof(htmlReaderService));
    }

    /// <summary>
    ///     Returns the src list of img tags.
    /// </summary>
    public IEnumerable<string> ExtractImagesLinks(string html)
    {
        var doc = _htmlReaderService.CreateHtmlDocument(html);
        var nodes = doc.DocumentNode.SelectNodes("//img[@src]");

        if (nodes == null)
        {
            yield break;
        }

        foreach (var image in nodes)
        {
            foreach (var attribute in image.Attributes.Where(attr
                         => attr.Name.Equals("src", StringComparison.OrdinalIgnoreCase)))
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
        var nodes = doc.DocumentNode.SelectNodes("//a[@href]");

        if (nodes == null)
        {
            yield break;
        }

        foreach (var image in nodes)
        {
            foreach (var attribute in image.Attributes.Where(attr
                         => attr.Name.Equals("href", StringComparison.OrdinalIgnoreCase)))
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
        var nodes = doc.DocumentNode.SelectNodes("//@background|//@lowsrc|//@src|//@href");

        if (nodes == null)
        {
            return doc.DocumentNode.OuterHtml;
        }

        foreach (var image in nodes)
        {
            foreach (var attribute in image.Attributes.Where(attr
                         => attr.Name.Equals("background", StringComparison.OrdinalIgnoreCase) ||
                            attr.Name.Equals("lowsrc", StringComparison.OrdinalIgnoreCase) ||
                            attr.Name.Equals("src", StringComparison.OrdinalIgnoreCase) ||
                            attr.Name.Equals("href", StringComparison.OrdinalIgnoreCase)))
            {
                var originalUrl = attribute.Value;

                if (string.IsNullOrWhiteSpace(originalUrl))
                {
                    attribute.Value = siteBaseUrl.CombineUrl(imageNotFoundPath);
                    _logger.LogWarning("Changed URL: '' to '{AttributeValue}'.", attribute.Value);

                    continue;
                }

                originalUrl = originalUrl.Trim();

                if (originalUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                    originalUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    if (!attribute.Value.Equals(originalUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Changed URL: '{AttributeValue}' to '{OriginalUrl}'.", attribute.Value,
                            originalUrl);

                        attribute.Value = originalUrl;
                    }

                    continue;
                }

                var idx = originalUrl.IndexOf("data:image/", StringComparison.OrdinalIgnoreCase);

                if (idx != -1)
                {
                    var newImage = originalUrl.Substring(idx);

                    if (!attribute.Value.Equals(newImage, StringComparison.OrdinalIgnoreCase))
                    {
                        attribute.Value = newImage;

                        _logger.LogWarning("Changed Image: '{OriginalUrl}' to '{AttributeValue}'.", originalUrl,
                            attribute.Value);
                    }

                    continue;
                }

                if (originalUrl.StartsWith("file:/", StringComparison.OrdinalIgnoreCase))
                {
                    attribute.Value = siteBaseUrl.CombineUrl(imageNotFoundPath);

                    _logger.LogWarning("Changed URL: '{OriginalUrl}' to '{AttributeValue}'.", originalUrl,
                        attribute.Value);

                    continue;
                }

                originalUrl = originalUrl.Replace("\\", "/", StringComparison.Ordinal).TrimStart('.').TrimStart('/')
                    .Trim();

                var newUrl = $"http://{originalUrl}";
                var (urlDomain, hasBestMatch) = newUrl.GetUrlDomain();

                if (!string.IsNullOrWhiteSpace(urlDomain) && hasBestMatch)
                {
                    attribute.Value = newUrl;
                    _logger.LogWarning("Changed URL: '{OriginalUrl}' to '{NewUrl}'.", originalUrl, newUrl);

                    continue;
                }

                newUrl = siteBaseUrl.CombineUrl(originalUrl);

                if (!newUrl.Equals(attribute.Value, StringComparison.OrdinalIgnoreCase))
                {
                    attribute.Value = newUrl;
                    _logger.LogWarning("Changed URL: '{OriginalUrl}' to '{NewUrl}'.", originalUrl, newUrl);
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
            throw new InvalidOperationException("`siteBaseUrl` is null.");
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
    public Task<string> GetUrlTitleAsync(string url)
        => GetUrlTitleAsync(new Uri(url));

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

        titleText = titleText.Trim().Replace(Environment.NewLine, " ", StringComparison.Ordinal)
            .Replace("\t", " ", StringComparison.Ordinal).Replace("\n", " ", StringComparison.Ordinal);

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

        return string.IsNullOrWhiteSpace(innerText) ? string.Empty : _htmlSpacesPattern.Replace(innerText, " ").Trim();
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