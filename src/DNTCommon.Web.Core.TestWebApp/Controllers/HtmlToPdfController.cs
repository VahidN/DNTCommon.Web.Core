using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class HtmlToPdfController(IHtmlToPdfGenerator htmlToPdfGenerator, IWebHostEnvironment webHostEnvironment)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        // Here you can find how define @page settings
        var inputHtmlFile = webHostEnvironment.WebRootPath.SafePathCombine("html-to-pdf-page-template", "page.html");

        var outputPdfFile = webHostEnvironment.WebRootPath.SafePathCombine("html-to-pdf-page-template", "page.pdf");

        var log = await htmlToPdfGenerator.GeneratePdfFromHtmlAsync(new HtmlToPdfGeneratorOptions
        {
            SourceHtmlFileOrUri = inputHtmlFile,
            OutputFilePath = outputPdfFile,
            DocumentMetadata = new PdfDocumentMetadata
            {
                Title = "Article 1",
                Subject = "post/1",
                Author = "VahidN",
                Creator = "dntips.ir",
                Keywords = "test1, test2"
            }
        });

        new FileExtensionContentTypeProvider().TryGetContentType(outputPdfFile, out var contentType);

        return PhysicalFile(outputPdfFile, contentType, fileDownloadName: "page.pdf");
    }
}
