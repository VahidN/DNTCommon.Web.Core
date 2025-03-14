using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class FeedResultController : Controller
{
    public IActionResult Index() => View();

    public IActionResult RssFeed()
    {
        var channel = GetWhatsNewFeedChannel();

        return new FeedResult<WhatsNewItemModel>(channel);
    }

    [Route(template: "/llms.txt")]
    public IActionResult Llms()
    {
        var channel = GetWhatsNewFeedChannel();

        return new LlmsTxtResult<WhatsNewItemModel>(channel);
    }

    [Route(template: "/llms-full.txt")]
    public IActionResult LlmsFull()
    {
        var channel = GetWhatsNewFeedChannel();

        return new LlmsFullTxtResult<WhatsNewItemModel>(channel);
    }

    private WhatsNewFeedChannel GetWhatsNewFeedChannel()
    {
        var rssItems = new List<WhatsNewItemModel>
        {
            new()
            {
                Title = "News 1",
                AuthorName = "VahidN",
                Content = "line1<br/>line2",
                Categories = new List<string>
                {
                    "MVC",
                    "ASP.NET"
                },
                Url = Url.Action(nameof(Article), typeof(FeedResultController).ControllerName(), new
                {
                    id = 1
                }, HttpContext.Request.Scheme),
                LastUpdatedTime = DateTimeOffset.UtcNow.AddDays(days: -1),
                PublishDate = DateTimeOffset.UtcNow.AddDays(days: -2)
            },
            new()
            {
                Title = "News 2 - خبر 2",
                AuthorName = "VahidN",
                Content = "line1<br/>سطر دوم",
                Categories = new List<string>
                {
                    "MVC",
                    "ASP.NET"
                },
                Url = Url.Action(nameof(Article), typeof(FeedResultController).ControllerName(), new
                {
                    id = 2
                }, HttpContext.Request.Scheme),
                LastUpdatedTime = DateTimeOffset.UtcNow.AddDays(days: -1),
                PublishDate = DateTimeOffset.UtcNow.AddDays(days: -2)
            }
        };

        var channel = new WhatsNewFeedChannel
        {
            FeedTitle = ".NET Core News",
            FeedDescription = "Latest .NET Core News",
            FeedCopyright = "DNT",
            FeedImageContentPath = "",
            FeedImageTitle = "",
            RssItems = rssItems,
            CultureName = "fa-IR"
        };

        return channel;
    }

    public IActionResult Article(int? id) => Content(id is null ? "" : id.Value.ToString(CultureInfo.InvariantCulture));
}

public class WhatsNewFeedChannel : FeedChannel<WhatsNewItemModel>
{
}

public class WhatsNewItemModel : FeedItem
{
    public int UserId { set; get; }
}
