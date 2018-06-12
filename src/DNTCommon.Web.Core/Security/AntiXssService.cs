using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// AntiXss Config
    /// </summary>
    public class AntiXssConfig
    {
        /// <summary>
        /// List of allowed HTML tags and their attributes
        /// </summary>
        public ValidHtmlTag[] ValidHtmlTags { set; get; }

        /// <summary>
        /// If an attribute's value contains one of these characters, it will be removed.
        /// </summary>
        public ISet<string> UnsafeAttributeValueCharacters { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Represents a valid HTML tag for the AntiXss
    /// </summary>
    public class ValidHtmlTag
    {
        /// <summary>
        /// A valid tag name
        /// </summary>
        public string Tag { set; get; }

        /// <summary>
        /// Valid tag's attributes
        /// </summary>
        public ISet<string> Attributes { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// SafeFile Download Service Extensions
    /// </summary>
    public static class AntiXssServiceExtensions
    {
        /// <summary>
        /// Adds IFileNameSanitizerService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddAntiXssService(this IServiceCollection services)
        {
            services.AddTransient<IAntiXssService, AntiXssService>();
            return services;
        }
    }

    /// <summary>
    /// Anti Xss Service
    /// </summary>
    public interface IAntiXssService
    {
        /// <summary>
        /// Takes raw HTML input and cleans against a whitelist
        /// </summary>
        /// <param name="html">Html source</param>
        /// <param name="allowDataAttributes">Allow HTML5 data attributes prefixed with data-</param>
        /// <returns>Clean output</returns>
        string GetSanitizedHtml(string html, bool allowDataAttributes = true);
    }

    /// <summary>
    /// Anti Xss Service
    /// </summary>
    public class AntiXssService : IAntiXssService
    {
        private readonly IOptionsSnapshot<AntiXssConfig> _antiXssConfig;
        private readonly ILogger<AntiXssService> _logger;
        private readonly IHtmlReaderService _htmlReaderService;

        /// <summary>
        /// Anti Xss Library
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
                throw new ArgumentNullException(nameof(antiXssConfig), "Please add AntiXssConfig to your appsettings.json file.");
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Takes raw HTML input and cleans against a whitelist
        /// </summary>
        /// <param name="html">Html source</param>
        /// <param name="allowDataAttributes">Allow HTML5 data attributes prefixed with data-</param>
        /// <returns>Clean output</returns>
        public string GetSanitizedHtml(string html, bool allowDataAttributes)
        {
            var parser = _htmlReaderService.ParseHtml(html);
            var whitelistTags = new HashSet<string>(_antiXssConfig.Value.ValidHtmlTags.Select(x => x.Tag.ToLowerInvariant()).ToArray(), StringComparer.OrdinalIgnoreCase);
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
                (node.Name == "pre" || node.Name == "code") &&
                !string.IsNullOrWhiteSpace(node.InnerHtml))
            {
                var decodedHtml = WebUtility.HtmlDecode(node.InnerHtml);
                var encodedHtml = WebUtility.HtmlEncode(decodedHtml);
                if (node.InnerHtml != encodedHtml)
                {
                    node.InnerHtml = encodedHtml;
                    _logger.LogWarning($"Fixed a non-encoded `{node.Name}` tag: `{node.OuterHtml}`.");
                }
            }
        }

        private bool cleanWhitespacesBetweenTags(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                if (string.IsNullOrWhiteSpace(node.InnerText) || string.IsNullOrEmpty(node.InnerText.Trim()))
                {
                    node.ParentNode?.RemoveChild(node);
                    return true;
                }
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
            (allowDataAttributes && attribute.Name != null && attribute.Name.StartsWith("data-", StringComparison.OrdinalIgnoreCase)) ||
             _antiXssConfig.Value.ValidHtmlTags.Any(tag => tag.Attributes.Contains(attribute.Name));
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
            attributeValue = attributeValue.Replace("\n", "")
                                           .Replace("\r", "")
                                           .Replace("\t", "")
                                           .Replace("`", "")
                                           .Replace("\0", "");

            foreach (var item in _antiXssConfig.Value.UnsafeAttributeValueCharacters)
            {
                if (attributeValue.IndexOf(item, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return (true, item);
                }
            }

            return (false, string.Empty);
        }
    }
}