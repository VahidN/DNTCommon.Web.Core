using DNTCommon.Web.Core.TestWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class AjaxExtensionsController(IAutoRegisterService autoRegisterService) : Controller
{
    public IActionResult Index() => View();

    [AjaxOnly]
    public IActionResult AjaxOnlyRequest()
        => Json(new
        {
            IsAjaxRequest = HttpContext.Request.IsAjaxRequest(),
            Data = autoRegisterService.GetDate()
        });
}
