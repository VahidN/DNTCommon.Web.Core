using System.Drawing;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class TextToImageController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult EmailToImage()
    {
        return new TextToImageResult("name@site.com",
                                        new TextToImageOptions
                                        {
                                            FontName = "Tahoma",
                                            FontSize = 24,
                                            FontColor = Color.Blue
                                        });
    }
}