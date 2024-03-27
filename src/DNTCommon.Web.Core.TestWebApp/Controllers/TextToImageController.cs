using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class TextToImageController : Controller
{
    public IActionResult Index()
        => View();

    public IActionResult EmailToImage()
        => new TextToImageResult( //"name@site.com + پنج به علاوه 2",
            "پنج به علاوه 2", new TextToImageOptions
            {
                FontName = "Tahoma",
                FontSize = 24,
                FontColor = SKColors.DarkBlue,
                CaptchaNoise = new CaptchaNoise(),
                AddDropShadow = true,
                ShadowColor = SKColors.BlueViolet
            });
}