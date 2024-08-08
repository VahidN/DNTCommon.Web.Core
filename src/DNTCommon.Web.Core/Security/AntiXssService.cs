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
    public AntiXssService(IHtmlReaderService htmlReaderService,
        IOptionsSnapshot<AntiXssConfig> antiXssConfig,
        ILogger<AntiXssService> logger)
    {
        _htmlReaderService = htmlReaderService ?? throw new ArgumentNullException(nameof(htmlReaderService));
        _antiXssConfig = antiXssConfig;

        _antiXssConfig = antiXssConfig ?? throw new ArgumentNullException(nameof(antiXssConfig));

        if (_antiXssConfig.Value == null || _antiXssConfig.Value.ValidHtmlTags == null)
        {
            throw new ArgumentNullException(nameof(antiXssConfig),
                message: "Please add AntiXssConfig to your appsettings.json file.");
        }

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Takes raw HTML input and cleans against a whitelist
    /// </summary>
    /// <param name="html">Html source</param>
    /// <param name="allowDataAttributes">Allow HTML5 data attributes prefixed with data-</param>
    /// <returns>Clean output</returns>
    public string GetSanitizedHtml(string? html, bool allowDataAttributes = true)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var parser = _htmlReaderService.ParseHtml(html);

        var whitelistTags =
            new HashSet<string>(_antiXssConfig.Value.ValidHtmlTags.Select(x => x.Tag.ToLowerInvariant()).ToArray(),
                StringComparer.OrdinalIgnoreCase);

        foreach (var node in parser.HtmlNodes.ToList())
        {
            FixCodeTag(node);

            if (CleanTags(whitelistTags, node))
            {
                continue;
            }

            if (CleanWhitespacesBetweenTags(node))
            {
                continue;
            }

            if (CleanComments(node))
            {
                continue;
            }

            CleanAttributes(node, allowDataAttributes);
        }

        return parser.HtmlDocument.DocumentNode.OuterHtml;
    }

    private void FixCodeTag(HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Element &&
            (string.Equals(node.Name, b: "pre", StringComparison.Ordinal) ||
             string.Equals(node.Name, b: "code", StringComparison.Ordinal)) &&
            !string.IsNullOrWhiteSpace(node.InnerHtml))
        {
            var decodedHtml = WebUtility.HtmlDecode(node.InnerHtml);
            var encodedHtml = WebUtility.HtmlEncode(decodedHtml);

            if (!string.Equals(node.InnerHtml, encodedHtml, StringComparison.Ordinal))
            {
                node.InnerHtml = encodedHtml;

                _logger.LogWarning(message: "Fixed a non-encoded `{NodeName}` tag: `{NodeOuterHtml}`.", node.Name,
                    node.OuterHtml);
            }

            node.InnerHtml = node.InnerHtml.Trim();

            node.SetAttributeValue(name: "dir", value: "ltr");

            node.SetAttributeValue(name: "style",
                value: "white-space: pre-wrap; overflow: auto; word-break: break-word; text-align: left;");
        }
    }

    private static bool CleanWhitespacesBetweenTags(HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Text &&
            (string.IsNullOrWhiteSpace(node.InnerText) || string.IsNullOrEmpty(node.InnerText.Trim())))
        {
            node.ParentNode?.RemoveChild(node);

            return true;
        }

        return false;
    }

    private void CleanAttributes(HtmlNode node, bool allowDataAttributes)
    {
        if (!node.HasAttributes)
        {
            return;
        }

        foreach (var attribute in node.Attributes.ToList())
        {
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                _logger.LogWarning(message: "Removed an empty attribute: `{AttributeName}` from `{NodeOuterHtml}`.",
                    attribute.Name, node.OuterHtml);

                attribute.Remove();

                continue;
            }

            if (!IsAllowedAttribute(attribute, allowDataAttributes))
            {
                _logger.LogWarning(message: "Removed a not valid attribute: `{AttributeName}` from `{NodeOuterHtml}`.",
                    attribute.Name, node.OuterHtml);

                attribute.Remove();

                continue;
            }

            attribute.Value = attribute.Value.Trim();

            var result = CheckAttributeValue(attribute.Value);

            if (result.HasUnsafeValue)
            {
                _logger.LogWarning(
                    message:
                    "Removed an unsafe[`{ResultUnsafeItem}`] attribute: `{AttributeName}` from `{NodeOuterHtml}`.",
                    result.UnsafeItem, attribute.Name, node.OuterHtml);

                attribute.Remove();

                continue;
            }

            result = CheckAttributeValue(attribute.DeEntitizeValue);

            if (result.HasUnsafeValue)
            {
                _logger.LogWarning(
                    message:
                    "Removed an unsafe[`{ResultUnsafeItem}`] attribute: `{AttributeName}` from `{NodeOuterHtml}`.",
                    result.UnsafeItem, attribute.Name, node.OuterHtml);

                attribute.Remove();
            }
        }
    }

    private bool IsAllowedAttribute(HtmlAttribute attribute, bool allowDataAttributes)
        => (allowDataAttributes &&
            attribute.Name?.StartsWith(value: "data-", StringComparison.OrdinalIgnoreCase) == true) ||
           _antiXssConfig.Value.ValidHtmlTags.Any(tag
               => attribute.Name != null && tag.Attributes.Contains(attribute.Name, StringComparer.OrdinalIgnoreCase));

    private bool CleanComments(HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Comment)
        {
            _logger.LogWarning(message: "Removed a comment: `{NodeOuterHtml}`.", node.OuterHtml);
            node.ParentNode?.RemoveChild(node);

            return true;
        }

        return false;
    }

    private bool CleanTags(HashSet<string> whitelistTags, HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Element && !whitelistTags.Contains(node.Name))
        {
            _logger.LogWarning(message: "Removed a not valid tag: `{NodeOuterHtml}`.", node.OuterHtml);
            node.ParentNode?.RemoveChild(node);

            return true;
        }

        return false;
    }

    private (bool HasUnsafeValue, string UnsafeItem) CheckAttributeValue(string attributeValue)
    {
        attributeValue = attributeValue.Replace(oldValue: "\n", newValue: "", StringComparison.Ordinal)
            .Replace(oldValue: "\r", newValue: "", StringComparison.Ordinal)
            .Replace(oldValue: "\t", newValue: "", StringComparison.Ordinal)
            .Replace(oldValue: "`", newValue: "", StringComparison.Ordinal)
            .Replace(oldValue: "\0", newValue: "", StringComparison.Ordinal);

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