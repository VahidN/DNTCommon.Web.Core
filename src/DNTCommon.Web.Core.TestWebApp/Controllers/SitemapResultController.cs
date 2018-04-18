using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers
{
    public class SitemapResultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("/sitemap.xml")]
        public IActionResult Sitemap()
        {
            var items = new List<SitemapItem>
            {
                new SitemapItem
                {
                    Url = Url.Action(nameof(Article), typeof(SitemapResultController).ControllerName(), new { id = 1 }, this.HttpContext.Request.Scheme),
                    LastUpdatedTime = DateTimeOffset.UtcNow.AddDays(-1)
                },
                new SitemapItem
                {
                    Url = Url.Action(nameof(Article), typeof(SitemapResultController).ControllerName(), new { id = 2 }, this.HttpContext.Request.Scheme),
                    LastUpdatedTime = DateTimeOffset.UtcNow.AddDays(-1)
                }
            };
            return new SitemapResult(items);
        }

        [Route("/robots.txt")] // Autodiscovery of /sitemap.xml
        public string RobotsTxt()
        {
            string siteMapFullUrl = Url.Action(nameof(Sitemap), typeof(SitemapResultController).ControllerName(), null, this.HttpContext.Request.Scheme);
            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow:");
            sb.AppendLine($"sitemap: {siteMapFullUrl}");
            return sb.ToString();
        }


        public IActionResult Article(int? id)
        {
            return Content(id == null ? "" : id.ToString());
        }
    }
}