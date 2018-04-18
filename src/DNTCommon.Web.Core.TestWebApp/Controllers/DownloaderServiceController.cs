using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class DownloaderServiceController : Controller
    {
        private readonly IDownloaderService _downloaderService;

        public DownloaderServiceController(IDownloaderService downloaderService)
        {
            _downloaderService = downloaderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DownloadFile()
        {
            var url = Url.Action(nameof(Index), typeof(DownloaderServiceController).ControllerName(), null, Request.Scheme);
            var result = await _downloaderService.DownloadPageAsync(url,
             new AutoRetriesPolicy
             {
                 MaxRequestAutoRetries = 2,
                 AutoRetriesDelay = TimeSpan.FromSeconds(3)
             });
            return Content(result.Data);
        }
    }
}