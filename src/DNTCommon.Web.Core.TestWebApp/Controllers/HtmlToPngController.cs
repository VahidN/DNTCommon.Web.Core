using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class HtmlToPngController(IHtmlToPngGenerator htmlToPngGenerator, IWebHostEnvironment webHostEnvironment)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        var outputPngFile = webHostEnvironment.WebRootPath.SafePathCombine("files", "test.png");

        var log = await htmlToPngGenerator.GeneratePngFromHtmlAsync(new HtmlToPngGeneratorOptions
        {
            SourceHtmlFileOrUri = "https://localhost:5001/TextToImage",
            OutputFilePath = outputPngFile
        });

        var isBlankImage = outputPngFile.IsBlankImage();

        var isBlankFile = webHostEnvironment.WebRootPath.SafePathCombine("files", "news-19169.jpg").IsBlankImage();

        var blankPixelsPercentage1 = webHostEnvironment.WebRootPath.SafePathCombine("files", "news-19598.jpg")
            .GetImageBlankPixelsPercentage();

        var blankPixelsPercentage2 = webHostEnvironment.WebRootPath.SafePathCombine("files", "news-19588.jpg")
            .GetImageBlankPixelsPercentage();

        var isPartiallyBlankImage = webHostEnvironment.WebRootPath.SafePathCombine("files", "news-19588.jpg")
            .IsPartiallyBlankImage(whitePixelsPercentage: 80);

        ViewBag.ImageSrc = "/files/test.png";

        return View();
    }
}
