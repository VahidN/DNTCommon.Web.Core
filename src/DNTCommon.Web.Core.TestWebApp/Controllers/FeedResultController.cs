using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class FeedResultController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult RssFeed()
    {
        var rssItems = new List<FeedItem>
        {
            new FeedItem
            {
                Title = "News 1",
                AuthorName = "VahidN",
                Content = "line1<br/>line2",
                Categories = new List<string> { "MVC", "ASP.NET" },
                Url = Url.Action(nameof(Article), typeof(FeedResultController).ControllerName(), new { id = 1 }, this.HttpContext.Request.Scheme),
                LastUpdatedTime = DateTimeOffset.UtcNow.AddDays(-1),
                PublishDate = DateTimeOffset.UtcNow.AddDays(-2)
            },
            new FeedItem
            {
                Title = "News 2 - خبر 2",
                AuthorName = "VahidN",
                Content = "line1<br/>سطر دوم",
                Categories = new List<string> { "MVC", "ASP.NET" },
                Url = Url.Action(nameof(Article), typeof(FeedResultController).ControllerName(), new { id = 2 }, this.HttpContext.Request.Scheme),
                LastUpdatedTime = DateTimeOffset.UtcNow.AddDays(-1),
                PublishDate = DateTimeOffset.UtcNow.AddDays(-2)
            }
        };
        var channel = new FeedChannel
        {
            FeedTitle = ".NET Core News",
            FeedDescription = "Latest .NET Core News",
            FeedCopyright = "DNT",
            FeedImageContentPath = "",
            FeedImageTitle = "",
            RssItems = rssItems,
            CultureName = "fa-IR"
        };
        return new FeedResult(channel);
    }

    public IActionResult Article(int? id)
    {
        return Content(id == null ? "" : id.ToString());
    }
}