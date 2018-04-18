using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class OpenSearchController : Controller
    {
        private readonly IHttpRequestInfoService _httpRequestInfoService;

        public OpenSearchController(IHttpRequestInfoService httpRequestInfoService)
        {
            _httpRequestInfoService = httpRequestInfoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RenderOpenSearch()
        {
            return new OpenSearchResult
            {
                ShortName = "My Site's Name",
                Description = "My Site's Contents Search",
                SearchForm = _httpRequestInfoService.GetBaseUrl(),
                FavIconUrl = $"{_httpRequestInfoService.GetBaseUrl()}/favicon.ico",
                SearchUrlTemplate = Url.Action(
                                            nameof(PublicSearch),
                                            typeof(OpenSearchController).ControllerName(),
                                            new { term = "searchTerms" },
                                            HttpContext.Request.Scheme)
            };
        }

        public IActionResult PublicSearch(string term)
        {
            return Content(term);
        }
    }
}