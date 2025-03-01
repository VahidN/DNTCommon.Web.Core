using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class SitemapResultController : Controller
{
    public IActionResult Index() => View();

    [Route(template: "/sitemap.xml")]
    public IActionResult Sitemap()
    {
        var items = new List<SitemapItem>
        {
            new()
            {
                Url = Url.Action(nameof(Article), typeof(SitemapResultController).ControllerName(), new
                {
                    id = 1
                }, HttpContext.Request.Scheme),
                LastUpdatedTime = DateTimeOffset.UtcNow.AddDays(days: -1)
            },
            new()
            {
                Url = Url.Action(nameof(Article), typeof(SitemapResultController).ControllerName(), new
                {
                    id = 2
                }, HttpContext.Request.Scheme),
                LastUpdatedTime = DateTimeOffset.UtcNow.AddDays(days: -1)
            }
        };

        return new SitemapResult(items);
    }

    [Route(template: "/robots.txt")] // Autodiscovery of /sitemap.xml
    public string RobotsTxt()
    {
        var siteMapFullUrl = Url.Action(nameof(Sitemap), typeof(SitemapResultController).ControllerName(), values: null,
            HttpContext.Request.Scheme);

        var sb = new StringBuilder();
        sb.AppendLine(value: "User-agent: *");
        sb.AppendLine(value: "Disallow:");
        sb.AppendLine(CultureInfo.InvariantCulture, $"sitemap: {siteMapFullUrl}");

        return sb.ToString();
    }

    public IActionResult Article(int? id) => Content(id is null ? "" : id.Value.ToString(CultureInfo.InvariantCulture));
}
