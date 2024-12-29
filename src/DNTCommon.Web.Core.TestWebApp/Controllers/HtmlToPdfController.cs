using System;
using System.IO;
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
        var inputHtmlFile = Path.Combine(webHostEnvironment.WebRootPath, path2: "html-to-pdf-page-template",
            path3: "page.html");

        var outputPdfFile = Path.Combine(webHostEnvironment.WebRootPath, path2: "html-to-pdf-page-template",
            path3: "page.pdf");

        var log = await htmlToPdfGenerator.GeneratePdfFromHtmlAsync(new HtmlToPdfGeneratorOptions
        {
            SourceHtmlFileOrUri = inputHtmlFile,
            OutputPdfFile = outputPdfFile,
            DocumentMetadata = new PdfDocumentMetadata
            {
                Title = "Article 1",
                Subject = "post/1",
                Author = "VahidN",
                Creator = "dntips.ir",
                Keywords = "test1, test2"
            }
        }, TimeSpan.FromMinutes(minutes: 1));

        new FileExtensionContentTypeProvider().TryGetContentType(outputPdfFile, out var contentType);

        return PhysicalFile(outputPdfFile, contentType, fileDownloadName: "page.pdf");
    }
}