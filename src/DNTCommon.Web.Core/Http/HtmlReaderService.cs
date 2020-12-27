using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Html Reader Service
    /// </summary>
    public class HtmlReaderService : IHtmlReaderService
    {
        private readonly ILogger<HtmlReaderService> _logger;

        /// <summary>
        /// Html Reader Service
        /// </summary>
        public HtmlReaderService(ILogger<HtmlReaderService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// ‍Creates a properly initialized new HtmlDocument.
        /// </summary>
        public HtmlDocument CreateHtmlDocument(string html)
        {
            var doc = new HtmlDocument
            {
                OptionCheckSyntax = true,
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = Encoding.UTF8
            };
            doc.LoadHtml(html);

            if (doc.ParseErrors?.Any() == true)
            {
                foreach (var error in doc.ParseErrors)
                {
                    _logger.LogWarning($"LoadHtml Error. SourceText: {error.SourceText} -> Code: {error.Code} -> Reason: {error.Reason}");
                }
            }

            return doc;
        }

        /// <summary>
        /// Parses an HTML document recursively.
        /// </summary>
        public (HtmlDocument HtmlDocument, IEnumerable<HtmlNode> HtmlNodes) ParseHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                throw new ArgumentNullException(nameof(html));
            }

            var doc = CreateHtmlDocument(html);
            return (doc, handleChildren(doc.DocumentNode.ChildNodes));
        }

        private IEnumerable<HtmlNode> handleChildren(HtmlNodeCollection nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Name.Equals("html", StringComparison.OrdinalIgnoreCase))
                {
                    var body = node.Element("body");
                    if (body != null)
                    {
                        foreach (var bodyNode in handleChildren(body.ChildNodes))
                        {
                            yield return bodyNode;
                        }
                    }
                }
                else
                {
                    yield return node;
                    foreach (var childNode in handleChildren(node.ChildNodes))
                    {
                        yield return childNode;
                    }
                }
            }
        }
    }
}