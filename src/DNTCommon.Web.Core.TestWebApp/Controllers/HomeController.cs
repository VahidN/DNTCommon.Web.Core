using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class HomeController : Controller
{
    private readonly IEnhancedStackTraceService _enhancedStackTrace;

    public HomeController(IEnhancedStackTraceService enhancedStackTrace)
    {
        _enhancedStackTrace = enhancedStackTrace;
    }

    public IActionResult Index()
    {
        var test = _enhancedStackTrace.GetCurrentStackTrace(skipFrame: declaringType => false);
        return View();
    }
}
