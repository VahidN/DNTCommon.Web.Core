using iTextSharp.text;
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
        if (filePath.IsEmpty() || !File.Exists(filePath))
        {
            return;
        }

        var inFile = File.ReadAllBytes(filePath);

        using var pdfDoc = new Document();
        using var fileStream = new FileStream(filePath, FileMode.Create);
        using var pdfWriter = new PdfSmartCopy(pdfDoc, fileStream);
        pdfDoc.Open();

        ApplySettings(metadata, pdfDoc, pdfWriter);

        using var reader = new PdfReader(inFile);

        var numberOfPages = reader.NumberOfPages;

        var page = 1;

        do
        {
            pdfWriter.AddPage(pdfWriter.GetImportedPage(reader, page));
            page++;
        }
        while (page <= numberOfPages);

        pdfWriter.FreeReader(reader);
        pdfDoc.Close();
    }

    private static void ApplySettings(PdfDocumentMetadata? metadata, Document pdfDoc, PdfWriter pdfWriter)
    {
        pdfWriter.ViewerPreferences = PdfWriter.PageModeUseOutlines;
        pdfWriter.SetFullCompression();

        if (metadata is not null)
        {
            if (!metadata.Keywords.IsEmpty())
            {
                pdfDoc.AddKeywords(metadata.Keywords);
            }

            if (!metadata.Author.IsEmpty())
            {
                pdfDoc.AddAuthor(metadata.Author);
            }

            if (!metadata.Title.IsEmpty())
            {
                pdfDoc.AddTitle(metadata.Title);
            }

            if (!metadata.Subject.IsEmpty())
            {
                pdfDoc.AddSubject(metadata.Subject);
            }

            if (!metadata.Creator.IsEmpty())
            {
                pdfDoc.AddCreator(metadata.Creator);
            }
        }

        pdfWriter.SetOpenAction(PdfAction.GotoLocalPage(page: 1,
            new PdfDestination(PdfDestination.XYZ, left: 0, pdfDoc.PageSize.Height, zoom: 1), pdfWriter));
    }
}