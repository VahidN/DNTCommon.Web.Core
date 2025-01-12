using iTextSharp.text.pdf;

namespace DNTCommon.Web.Core;

/// <summary>
///     PDF files manipulation utilities
/// </summary>
public static class PdfExtensions
{
    /// <summary>
    ///     Applies PdfDocumentMetadata to a given PDF file
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="metadata"></param>
    public static void AddMetadataToPdfFile(this string? filePath, PdfDocumentMetadata? metadata)
    {
        if (!filePath.FileExists())
        {
            return;
        }

        var inFile = File.ReadAllBytes(filePath);

        using var reader = new PdfReader(inFile);
        using var outStream = new FileStream(filePath, FileMode.Create);
        using var stamper = new PdfStamper(reader, outStream);

        if (metadata is not null)
        {
            if (!metadata.Keywords.IsEmpty())
            {
                stamper.AddKeywords(metadata.Keywords);
            }

            if (!metadata.Author.IsEmpty())
            {
                stamper.AddAuthor(metadata.Author);
            }

            if (!metadata.Title.IsEmpty())
            {
                stamper.AddTitle(metadata.Title);
            }

            if (!metadata.Subject.IsEmpty())
            {
                stamper.AddSubject(metadata.Subject);
            }

            if (!metadata.Creator.IsEmpty())
            {
                stamper.AddCreator(metadata.Creator);
            }
        }

        stamper.ViewerPreferences = PdfWriter.PageModeUseOutlines;
        stamper.SetFullCompression();
    }
}