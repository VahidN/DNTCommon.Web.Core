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
            var readerInfo = reader.Info;

            if (!metadata.Keywords.IsEmpty())
            {
                readerInfo[key: "Keywords"] = metadata.Keywords;
            }

            if (!metadata.Author.IsEmpty())
            {
                readerInfo[key: "Author"] = metadata.Author;
            }

            if (!metadata.Title.IsEmpty())
            {
                readerInfo[key: "Title"] = metadata.Title;
            }

            if (!metadata.Subject.IsEmpty())
            {
                readerInfo[key: "Subject"] = metadata.Subject;
            }

            if (!metadata.Creator.IsEmpty())
            {
                readerInfo[key: "Creator"] = metadata.Creator;
            }

            stamper.MoreInfo = readerInfo;
        }

        stamper.ViewerPreferences = PdfWriter.PageModeUseOutlines;
        stamper.SetFullCompression();
    }
}