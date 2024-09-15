using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class HtmlToPngController(IHtmlToPngGenerator htmlToPngGenerator, IWebHostEnvironment webHostEnvironment)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        var outputPngFile = Path.Combine(webHostEnvironment.WebRootPath, path2: "files", path3: "test.png");

        var log = await htmlToPngGenerator.GeneratePngFromHtmlAsync(new HtmlToPngGeneratorOptions
        {
            SourceHtmlFileOrUri = "https://localhost:5001/TextToImage",
            OutputPngFile = outputPngFile
        });

        var isBlankFile = Path.Combine(webHostEnvironment.WebRootPath, path2: "files", path3: "news-19169.jpg")
            .IsBlankImage();

        var isBlankImage = outputPngFile.IsBlankImage();

        ViewBag.ImageSrc = "/files/test.png";

        return View();
    }
}