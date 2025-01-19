namespace DNTCommon.Web.Core;

/// <summary>
///     HtmlToPdfGenerator Options
/// </summary>
public class HtmlToPdfGeneratorOptions
{
    /// <summary>
    ///     Source Uri
    /// </summary>
    public string? SourceHtmlFileOrUri { set; get; }

    /// <summary>
    ///     Output path
    /// </summary>
    public string? OutputPdfFile { set; get; }

    /// <summary>
    ///     Defines metadata information of the Document.
    /// </summary>
    public PdfDocumentMetadata? DocumentMetadata { set; get; }

    /// <summary>
    ///     If it's not specified, ChromeFinder.Find with try to find it!
    /// </summary>
    public string? ChromeExecutablePath { get; set; } = "";

    /// <summary>
    ///     Wait for exit.
    /// </summary>
    public TimeSpan? WaitForExit { set; get; }
}