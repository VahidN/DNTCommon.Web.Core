using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Html Helper Service Extensions
/// </summary>
public static class HtmlHelperServiceExtensions
{
    private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(value: 1);

    private static readonly Regex HtmlSpacesPattern =
        new(pattern: @"&nbsp;|&zwnj;|(\n)\s*", RegexOptions.Compiled, OneMinute);

    /// <summary>
    ///     Adds IHtmlHelperService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddHtmlHelperService(this IServiceCollection services)
    {
        services.AddScoped<IHtmlHelperService, HtmlHelperService>();

        return services;
    }

    /// <summary>
    ///     Returns the src list of img tags.
    /// </summary>
    public static IEnumerable<string> ExtractImagesLinks(this string html, ILogger? logger = null)
    {
        var doc = html.CreateHtmlDocument(logger);
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
    public static IEnumerable<string> ExtractLinks(this string html, ILogger? logger = null)
    {
        var doc = html.CreateHtmlDocument(logger);
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
    ///     Is this node an HTML image
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static bool IsImageNode([NotNullWhen(returnValue: true)] this HtmlNode? node)
        => node is not null && node.NodeType == HtmlNodeType.Element &&
           string.Equals(node.Name, b: "img", StringComparison.Ordinal);

    /// <summary>
    ///     Returns the src value of an HTML image element
    /// </summary>
    public static string? GetSrcAttributeValue(this HtmlNode? node)
        => node?.Attributes.FirstOrDefault(attr => attr.Name.Equals(value: "src", StringComparison.OrdinalIgnoreCase))
            ?.Value?.Trim();

    /// <summary>
    ///     Returns the src value of an HTML image element
    /// </summary>
    public static HtmlAttribute? GetSrcAttribute(this HtmlNode? node)
        => node?.Attributes.FirstOrDefault(attr => attr.Name.Equals(value: "src", StringComparison.OrdinalIgnoreCase));

    /// <summary>
    ///     Is Base64 encoded image?
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public static bool IsBase64EncodedImage([NotNullWhen(returnValue: true)] this string? src)
        => src?.StartsWith(value: "data:image/", StringComparison.OrdinalIgnoreCase) == true;

    /// <summary>
    ///     Returns the image  bytes of the Base64 encoded image
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public static byte[]? GetBase64EncodedImageData([NotNullIfNotNull(nameof(src))] this string? src)
    {
        if (!src.IsBase64EncodedImage())
        {
            return null;
        }

        // data:[<MIME-type>][;charset=<encoding>][;base64],<data>
        var base64Data = src.Substring(src.IndexOf(value: ',', StringComparison.OrdinalIgnoreCase) + 1);

        return Convert.FromBase64String(base64Data);
    }

    /// <summary>
    ///     Extracts the given HTML page's title.
    /// </summary>
    public static string GetHtmlPageTitle(this string html, ILogger? logger = null)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var doc = html.CreateHtmlDocument(logger);
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
    public static string RemoveHtmlTags(this string html, ILogger? logger = null)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var doc = html.CreateHtmlDocument(logger);
        var innerText = doc.DocumentNode.InnerText;

        return string.IsNullOrWhiteSpace(innerText)
            ? string.Empty
            : HtmlSpacesPattern.Replace(innerText, replacement: " ").Trim();
    }

    /// <summary>
    ///     An enhanced version of HttpUtility.HtmlEncode method
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string? FullHtmlEncode(this string? text)
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
    public static IEnumerable<HtmlAttributeCollection> SelectNodes(this string html,
        string xpath,
        ILogger? logger = null)
    {
        var doc = html.CreateHtmlDocument(logger);
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