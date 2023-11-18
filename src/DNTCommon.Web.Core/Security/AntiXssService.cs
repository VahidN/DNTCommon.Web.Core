using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

/// <summary>
///     Anti Xss Service
/// </summary>
public class AntiXssService : IAntiXssService
{
    private readonly IOptionsSnapshot<AntiXssConfig> _antiXssConfig;
    private readonly IHtmlReaderService _htmlReaderService;
    private readonly ILogger<AntiXssService> _logger;

    /// <summary>
    ///     Anti Xss Library
    /// </summary>
    public AntiXssService(
        IHtmlReaderService htmlReaderService,
        IOptionsSnapshot<AntiXssConfig> antiXssConfig,
        ILogger<AntiXssService> logger)
    {
        _htmlReaderService = htmlReaderService ?? throw new ArgumentNullException(nameof(htmlReaderService));
        _antiXssConfig = antiXssConfig;

        _antiXssConfig = antiXssConfig ?? throw new ArgumentNullException(nameof(antiXssConfig));
        if (_antiXssConfig.Value == null || _antiXssConfig.Value.ValidHtmlTags == null)
        {
            throw new ArgumentNullException(nameof(antiXssConfig),
                                            "Please add AntiXssConfig to your appsettings.json file.");
        }

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Takes raw HTML input and cleans against a whitelist
    /// </summary>
    /// <param name="html">Html source</param>
    /// <param name="allowDataAttributes">Allow HTML5 data attributes prefixed with data-</param>
    /// <returns>Clean output</returns>
    public string GetSanitizedHtml(string html, bool allowDataAttributes = true)
    {
        var parser = _htmlReaderService.ParseHtml(html);
        var whitelistTags =
            new HashSet<string>(_antiXssConfig.Value.ValidHtmlTags.Select(x => x.Tag.ToLowerInvariant()).ToArray(),
                                StringComparer.OrdinalIgnoreCase);
        foreach (var node in parser.HtmlNodes.ToList())
        {
            fixCodeTag(node);

            if (cleanTags(whitelistTags, node))
            {
                continue;
            }

            if (cleanWhitespacesBetweenTags(node))
            {
                continue;
            }

            if (cleanComments(node))
            {
                continue;
            }

            cleanAttributes(node, allowDataAttributes);
        }

        return parser.HtmlDocument.DocumentNode.OuterHtml;
    }

    private void fixCodeTag(HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Element &&
            (string.Equals(node.Name, "pre", StringComparison.Ordinal)
             || string.Equals(node.Name, "code", StringComparison.Ordinal))
            && !string.IsNullOrWhiteSpace(node.InnerHtml))
        {
            var decodedHtml = WebUtility.HtmlDecode(node.InnerHtml);
            var encodedHtml = WebUtility.HtmlEncode(decodedHtml);
            if (!string.Equals(node.InnerHtml, encodedHtml, StringComparison.Ordinal))
            {
                node.InnerHtml = encodedHtml;
                _logger.LogWarning($"Fixed a non-encoded `{node.Name}` tag: `{node.OuterHtml}`.");
            }
        }
    }

    private static bool cleanWhitespacesBetweenTags(HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Text
            && (string.IsNullOrWhiteSpace(node.InnerText)
                || string.IsNullOrEmpty(node.InnerText.Trim())))
        {
            node.ParentNode?.RemoveChild(node);
            return true;
        }

        return false;
    }

    private void cleanAttributes(HtmlNode node, bool allowDataAttributes)
    {
        if (!node.HasAttributes)
        {
            return;
        }

        foreach (var attribute in node.Attributes.ToList())
        {
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                _logger.LogWarning($"Removed an empty attribute: `{attribute.Name}` from `{node.OuterHtml}`.");
                attribute.Remove();
                continue;
            }

            if (!isAllowedAttribute(attribute, allowDataAttributes))
            {
                _logger.LogWarning($"Removed a not valid attribute: `{attribute.Name}` from `{node.OuterHtml}`.");
                attribute.Remove();
                continue;
            }

            attribute.Value = attribute.Value.Trim();

            var result = checkAttributeValue(attribute.Value);
            if (result.HasUnsafeValue)
            {
                _logger.LogWarning($"Removed an unsafe[`{result.UnsafeItem}`] attribute: `{attribute.Name}` from `{node.OuterHtml}`.");
                attribute.Remove();
                continue;
            }

            result = checkAttributeValue(attribute.DeEntitizeValue);
            if (result.HasUnsafeValue)
            {
                _logger.LogWarning($"Removed an unsafe[`{result.UnsafeItem}`] attribute: `{attribute.Name}` from `{node.OuterHtml}`.");
                attribute.Remove();
            }
        }
    }

    private bool isAllowedAttribute(HtmlAttribute attribute, bool allowDataAttributes)
    {
        return
            (allowDataAttributes && attribute.Name?.StartsWith("data-", StringComparison.OrdinalIgnoreCase) == true)
            || _antiXssConfig.Value.ValidHtmlTags.Any(tag => attribute.Name != null &&
                                                             tag.Attributes.Contains(attribute.Name,
                                                              StringComparer.OrdinalIgnoreCase));
    }

    private bool cleanComments(HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Comment)
        {
            _logger.LogWarning($"Removed a comment: `{node.OuterHtml}`.");
            node.ParentNode?.RemoveChild(node);
            return true;
        }

        return false;
    }

    private bool cleanTags(HashSet<string> whitelistTags, HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Element && !whitelistTags.Contains(node.Name))
        {
            _logger.LogWarning($"Removed a not valid tag: `{node.OuterHtml}`.");
            node.ParentNode?.RemoveChild(node);
            return true;
        }

        return false;
    }

    private (bool HasUnsafeValue, string UnsafeItem) checkAttributeValue(string attributeValue)
    {
        attributeValue = attributeValue.Replace("\n", "", StringComparison.Ordinal)
                                       .Replace("\r", "", StringComparison.Ordinal)
                                       .Replace("\t", "", StringComparison.Ordinal)
                                       .Replace("`", "", StringComparison.Ordinal)
                                       .Replace("\0", "", StringComparison.Ordinal);

        foreach (var item in _antiXssConfig.Value.UnsafeAttributeValueCharacters)
        {
            if (attributeValue.Contains(item, StringComparison.OrdinalIgnoreCase))
            {
                return (true, item);
            }
        }

        return (false, string.Empty);
    }
}