using System.Threading.Tasks;
using DNTCommon.Web.Core.TestWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class ViewRendererServiceController : Controller
{
    private readonly IViewRendererService _viewRendererService;

    public ViewRendererServiceController(IViewRendererService viewRendererService)
    {
        _viewRendererService = viewRendererService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Render()
    {
        var content = await _viewRendererService.RenderViewToStringAsync(
            viewNameOrPath: "~/Views/EmailTemplates/_Template1.cshtml",
            model: new EmailTemplateViewModel
            {
                EmailSignature = "DNT"
            });
        return Content(content);
    }
}