using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class CommonHttpClientFactoryController : Controller
{
    private readonly ICommonHttpClientFactory _httpClientFactory;

    public CommonHttpClientFactoryController(ICommonHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> DownloadFile()
    {
        var host = new Uri("http://localhost:5000");
        var httpClient = _httpClientFactory.GetOrCreate(host);
        var responseMessage = await httpClient.GetAsync("CommonHttpClientFactory/Index");
        var responseContent = await responseMessage.Content.ReadAsStringAsync();
        return Content(responseContent);
    }
}