using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class RedirectUrlFinderServiceController : Controller
{
    private readonly IRedirectUrlFinderService _redirectUrlFinderService;

    public RedirectUrlFinderServiceController(IRedirectUrlFinderService redirectUrlFinderService)
    {
        _redirectUrlFinderService = redirectUrlFinderService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.InitialUrl = Url.Action(nameof(Start), typeof(RedirectUrlFinderServiceController).ControllerName(), null, Request.Scheme);
        ViewBag.FinalUrl = await _redirectUrlFinderService.GetRedirectUrlAsync(ViewBag.InitialUrl);
        return View();
    }

    public IActionResult Start()
    {
        return RedirectToAction(nameof(Redirect1));
    }

    public IActionResult Redirect1()
    {
        return RedirectToActionPermanent(nameof(Redirect2));
    }

    public IActionResult Redirect2()
    {
        return RedirectToAction(nameof(End));
    }

    public IActionResult End()
    {
        return Content("This is the actual content!");
    }
}