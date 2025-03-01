using System;
using System.Threading.Tasks;
using DNTCommon.Web.Core.TestWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class HttpRequestInfoController(IHttpRequestInfoService httpRequestInfoService, IUAParserService uaParserService)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        var isSpiderClient = await uaParserService.IsSpiderClientAsync(HttpContext);

        return View(new HttpRequestInfoModel
        {
            IsSpiderClient = isSpiderClient
        });
    }

    [HttpPost]
    [EnableReadableBodyStream]
    public async Task<IActionResult> Index([FromBody] RoleViewModel model)
    {
        var requestBody = string.Empty;

        if (Request.IsAjaxRequest() &&
            Request.ContentType?.Contains(value: "application/json", StringComparison.OrdinalIgnoreCase) == true)
        {
            //var roleModel = await _httpRequestInfoService.DeserializeRequestJsonBodyAsAsync<RoleViewModel>();
            requestBody = await httpRequestInfoService.ReadRequestBodyAsStringAsync();
        }

        return Content(requestBody);
    }
}

public class HttpRequestInfoModel
{
    public bool IsSpiderClient { set; get; }
}
