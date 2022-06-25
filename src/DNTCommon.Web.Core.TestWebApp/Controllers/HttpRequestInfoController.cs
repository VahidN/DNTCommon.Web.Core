using System.Threading.Tasks;
using DNTCommon.Web.Core.TestWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class HttpRequestInfoController : Controller
{
    private readonly IHttpRequestInfoService _httpRequestInfoService;

    public HttpRequestInfoController(IHttpRequestInfoService httpRequestInfoService)
    {
        _httpRequestInfoService = httpRequestInfoService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [EnableReadableBodyStream]
    public async Task<IActionResult> Index([FromBody] RoleViewModel model)
    {
        var requestBody = string.Empty;
        if (Request.IsAjaxRequest() && Request.ContentType.Contains("application/json"))
        {
            //var roleModel = await _httpRequestInfoService.DeserializeRequestJsonBodyAsAsync<RoleViewModel>();
            requestBody = await _httpRequestInfoService.ReadRequestBodyAsStringAsync();
        }
        return Content(requestBody);
    }
}