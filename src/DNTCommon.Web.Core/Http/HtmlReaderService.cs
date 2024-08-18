using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Html Reader Service
/// </summary>
public class HtmlReaderService : IHtmlReaderService
{
    private readonly ILogger<HtmlReaderService> _logger;

    /// <summary>
    ///     Html Reader Service
    /// </summary>
    public HtmlReaderService(ILogger<HtmlReaderService> logger)
        => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Creates a properly initialized new HtmlDocument.
    /// </summary>
    public HtmlDocument CreateHtmlDocument(string html) => html.CreateHtmlDocument(_logger);

    /// <summary>
    ///     Parses an HTML document recursively.
    /// </summary>
    public (HtmlDocument HtmlDocument, IEnumerable<HtmlNode> HtmlNodes) ParseHtml(string html)
        => html.ParseHtml(_logger);
}