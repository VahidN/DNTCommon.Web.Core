using HtmlAgilityPack;

namespace DNTCommon.Web.Core;

/// <summary>
///     Html Reader Service
/// </summary>
public interface IHtmlReaderService
{
    /// <summary>
    ///     ‍Creates a properly initialized new HtmlDocument.
    /// </summary>
    HtmlDocument CreateHtmlDocument(string html);

    /// <summary>
    ///     Parses an HTML document recursively.
    /// </summary>
    (HtmlDocument HtmlDocument, IEnumerable<HtmlNode> HtmlNodes) ParseHtml(string html);
}