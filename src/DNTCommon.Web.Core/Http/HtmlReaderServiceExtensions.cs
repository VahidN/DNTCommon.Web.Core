using System.Text;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     HtmlReaderService Extensions
/// </summary>
public static class HtmlReaderServiceExtensions
{
    /// <summary>
    ///     Adds IHtmlReaderService to IServiceCollection
    /// </summary>
    public static IServiceCollection AddHtmlReaderService(this IServiceCollection services)
    {
        services.AddTransient<IHtmlReaderService, HtmlReaderService>();

        return services;
    }

    /// <summary>
    ///     Creates a properly initialized new HtmlDocument.
    /// </summary>
    public static HtmlDocument CreateHtmlDocument(this string html, ILogger? logger = null)
    {
        var doc = new HtmlDocument
        {
            OptionCheckSyntax = true,
            OptionFixNestedTags = true,
            OptionAutoCloseOnEnd = true,
            OptionDefaultStreamEncoding = Encoding.UTF8
        };

        doc.LoadHtml(html);

        if (logger is not null && doc.ParseErrors?.Any() == true)
        {
            foreach (var error in doc.ParseErrors)
            {
                logger.LogInformation(
                    message:
                    "LoadHtml Error. SourceText: {ErrorSourceText} -> Code: {ErrorCode} -> Reason: {ErrorReason}",
                    error.SourceText, error.Code, error.Reason);
            }
        }

        return doc;
    }

    /// <summary>
    ///     Parses an HTML document recursively.
    /// </summary>
    public static (HtmlDocument HtmlDocument, IEnumerable<HtmlNode> HtmlNodes) ParseHtml(this string html,
        ILogger? logger = null)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            throw new ArgumentNullException(nameof(html));
        }

        var doc = CreateHtmlDocument(html, logger);

        return (doc, HandleChildren(doc.DocumentNode.ChildNodes));
    }

    private static IEnumerable<HtmlNode> HandleChildren(HtmlNodeCollection nodes)
    {
        foreach (var node in nodes)
        {
            if (node.Name.Equals(value: "html", StringComparison.OrdinalIgnoreCase))
            {
                var body = node.Element(name: "body");

                if (body != null)
                {
                    foreach (var bodyNode in HandleChildren(body.ChildNodes))
                    {
                        yield return bodyNode;
                    }
                }
            }
            else
            {
                yield return node;

                foreach (var childNode in HandleChildren(node.ChildNodes))
                {
                    yield return childNode;
                }
            }
        }
    }
}