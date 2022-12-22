using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class TextToImageController : Controller
{
    public IActionResult Index() => View();

    public IActionResult EmailToImage() =>
        new TextToImageResult("name@site.com",
                              new TextToImageOptions
                              {
                                  FontName = "Tahoma",
                                  FontSize = 24,
                                  FontColor = SKColors.Blue,
                              });
}