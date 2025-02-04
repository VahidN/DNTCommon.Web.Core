using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

/// <summary>
///     Anti Xss Service
/// </summary>
public class AntiXssService : IAntiXssService
{
    private readonly IHtmlReaderService _htmlReaderService;
    private readonly ILogger<AntiXssService> _logger;
    private readonly IReplaceRemoteImagesService _remoteImagesService;
    private AntiXssConfig _antiXssConfig;

    /// <summary>
    ///     Anti Xss Library
    /// </summary>
    public AntiXssService(IHtmlReaderService htmlReaderService,
        IOptionsMonitor<AntiXssConfig> antiXssConfigMonitor,
        IReplaceRemoteImagesService remoteImagesService,
        ILogger<AntiXssService> logger)
    {
        ArgumentNullException.ThrowIfNull(antiXssConfigMonitor);

        _htmlReaderService = htmlReaderService ?? throw new ArgumentNullException(nameof(htmlReaderService));
        _antiXssConfig = antiXssConfigMonitor.CurrentValue;
        antiXssConfigMonitor.OnChange(options => { _antiXssConfig = options; });

        _remoteImagesService = remoteImagesService;

        if (_antiXssConfig?.ValidHtmlTags == null)
        {
            throw new ArgumentNullException(nameof(antiXssConfigMonitor),
                message: "Please add AntiXssConfig to your appsettings.json file.");
        }

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Takes raw HTML input and cleans against a whitelist
    /// </summary>
    /// <param name="html">Html source</param>
    /// <param name="allowDataAttributes">Allow HTML5 data attributes prefixed with data-</param>
    /// <param name="remoteImagesOptions"></param>
    /// <param name="htmlModificationRules"></param>
    /// <returns>Clean output</returns>
    public string GetSanitizedHtml(string? html,
        bool allowDataAttributes = true,
        FixRemoteImagesOptions? remoteImagesOptions = null,
        HtmlModificationRules? htmlModificationRules = null)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var (htmlDocument, htmlNodes) = _htmlReaderService.ParseHtml(html);

        var whitelistTags =
            new HashSet<string>(_antiXssConfig.ValidHtmlTags.Select(x => x.Tag.ToLowerInvariant()).ToArray(),
                StringComparer.OrdinalIgnoreCase);

        var emptyLinesCount = 0;

        foreach (var node in htmlNodes.ToList())
        {
            FixCodeTag(node, htmlModificationRules);

            _remoteImagesService.FixRemoteImages(node, remoteImagesOptions);

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

            ConvertPToDiv(node, htmlModificationRules);

            RemoveRelAndTargetFromInternalUrls(node, htmlModificationRules);

            RemoveConsecutiveEmptyLines(node, htmlModificationRules, ref emptyLinesCount);
        }

        return htmlDocument.DocumentNode.OuterHtml;
    }

    private void RemoveConsecutiveEmptyLines(HtmlNode node,
        HtmlModificationRules? htmlModificationRules,
        ref int emptyLinesCount)
    {
        if (htmlModificationRules?.RemoveConsecutiveEmptyLines != true || node.NodeType != HtmlNodeType.Element)
        {
            return;
        }

        string[] nodesToInspect = ["div", "p", "br", "span"];

        if (!nodesToInspect.Any(nodeToInspect
                => string.Equals(node.Name, nodeToInspect, StringComparison.OrdinalIgnoreCase)))
        {
            emptyLinesCount = 0;

            return;
        }

        if (node.ParentNode is null)
        {
            return;
        }

        if (node.InnerText.IsEmpty())
        {
            emptyLinesCount++;
        }
        else
        {
            emptyLinesCount = 0;
        }

        if (emptyLinesCount > htmlModificationRules.MaxAllowedConsecutiveEmptyLines)
        {
            _logger.LogInformation(message: "Removed an empty line: `{NodeOuterHtml}`.", node.OuterHtml);
            node.RemoveAll();
            node.Remove();
        }
    }

    private void RemoveRelAndTargetFromInternalUrls(HtmlNode node, HtmlModificationRules? htmlModificationRules)
    {
        if (htmlModificationRules?.RemoveRelAndTargetFromInternalUrls != true ||
            node.NodeType != HtmlNodeType.Element || htmlModificationRules.HostUri == null)
        {
            return;
        }

        if (string.Equals(node.Name, b: "a", StringComparison.Ordinal))
        {
            var href = node.Attributes
                .FirstOrDefault(attr => attr.Name.Equals(value: "href", StringComparison.OrdinalIgnoreCase))
                ?.Value;

            if (href.IsEmpty())
            {
                _logger.LogInformation(message: "Removed an empty link: `{NodeOuterHtml}`.", node.OuterHtml);
                node.Remove();

                return;
            }

            if (!href.HaveTheSameDomain(htmlModificationRules.HostUri))
            {
                return;
            }

            foreach (var attribute in node.Attributes.ToList())
            {
                if (attribute.Value.IsEmpty())
                {
                    continue;
                }

                if (string.Equals(attribute.Name, b: "rel", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(attribute.Name, b: "target", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation(
                        message: "Removed `rel` or `target` attribute from the link: `{NodeOuterHtml}`.",
                        node.OuterHtml);

                    attribute.Remove();
                }
            }
        }
    }

    private static void ConvertPToDiv(HtmlNode node, HtmlModificationRules? htmlModificationRules)
    {
        if (htmlModificationRules?.ConvertPToDiv != true || node.NodeType != HtmlNodeType.Element)
        {
            return;
        }

        if (string.Equals(node.Name, b: "p", StringComparison.Ordinal))
        {
            node.Name = "div";

            if (string.IsNullOrWhiteSpace(node.InnerHtml?.Trim()))
            {
                node.InnerHtml = "<br>";
            }
        }

        if (string.Equals(node.Name, b: "div", StringComparison.Ordinal) &&
            string.IsNullOrWhiteSpace(node.InnerHtml?.Trim()))
        {
            node.InnerHtml = "<br>";
        }
    }

    private void FixCodeTag(HtmlNode node, HtmlModificationRules? rules)
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
            node.SetAttributeValue(name: "style", rules?.PreCodeStyles ?? "");
        }
    }

    private bool CleanWhitespacesBetweenTags(HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Text &&
            (string.IsNullOrWhiteSpace(node.InnerText) || string.IsNullOrEmpty(node.InnerText.Trim())))
        {
            _logger.LogInformation(
                message: "Cleaned the whitespaces between tags. InnerStartIndex:`{InnerStartIndex}`.",
                node.InnerStartIndex);

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
                _logger.LogInformation(message: "Removed an empty attribute: `{AttributeName}` from `{NodeOuterHtml}`.",
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

            if (attribute.Value.HasFullWidthChar() || attribute.Value.HasSmallCapitalChar())
            {
                _logger.LogWarning(
                    message:
                    "Removed an unsafe[`{ResultUnsafeItem}`] attribute with a bad width char: `{AttributeName}` from `{NodeOuterHtml}`.",
                    attribute.Value, attribute.Name, node.OuterHtml);

                attribute.Remove();

                continue;
            }

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
           _antiXssConfig.ValidHtmlTags.Any(tag => attribute.Name != null && tag.Attributes.Contains(attribute.Name));

    private bool CleanComments(HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Comment)
        {
            _logger.LogInformation(message: "Removed a comment: `{NodeOuterHtml}`.", node.OuterHtml);
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

        foreach (var item in _antiXssConfig.UnsafeAttributeValueCharacters)
        {
            if (attributeValue.Contains(item, StringComparison.OrdinalIgnoreCase))
            {
                return (true, item);
            }
        }

        return (false, string.Empty);
    }
}