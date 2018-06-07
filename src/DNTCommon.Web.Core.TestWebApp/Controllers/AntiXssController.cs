using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class AntiXssController : Controller
    {
        private readonly IAntiXssService _antiXssService;

        public AntiXssController(IAntiXssService antiXssService)
        {
            _antiXssService = antiXssService;
        }

        public IActionResult Index()
        {
            ViewBag.Text = _antiXssService.GetSanitizedHtml("<A HREF=\"http://www.codeplex.com?url=¼script¾alert(¢XSS¢)¼/script¾\">XSS</A>");
            return View();
        }
    }
}