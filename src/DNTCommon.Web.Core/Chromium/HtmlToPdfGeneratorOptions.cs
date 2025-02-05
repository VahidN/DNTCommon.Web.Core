namespace DNTCommon.Web.Core;

/// <summary>
///     HtmlToPdfGenerator Options
/// </summary>
public class HtmlToPdfGeneratorOptions : BaseChromiumGeneratorOptions
{
    /// <summary>
    ///     Defines metadata information of the Document.
    /// </summary>
    public PdfDocumentMetadata? DocumentMetadata { set; get; }

    /// <inheritdoc />
    public override string ToString()
        => $"SourceHtmlFileOrUri:`{SourceHtmlFileOrUri}`, OutputPdfFile:`{OutputFilePath}`, WaitForExit:`{WaitForExit}`, DocumentMetadata:`{DocumentMetadata}`.";
}