using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Html Reader Service
/// </summary>
public class HtmlReaderService(ILogger<HtmlReaderService> logger) : IHtmlReaderService
{
    /// <summary>
    ///     Creates a properly initialized new HtmlDocument.
    /// </summary>
    public HtmlDocument CreateHtmlDocument(string html) => html.CreateHtmlDocument(logger);

    /// <summary>
    ///     Parses an HTML document recursively.
    /// </summary>
    public (HtmlDocument HtmlDocument, IEnumerable<HtmlNode> HtmlNodes) ParseHtml(string html)
        => html.ParseHtml(logger);
}