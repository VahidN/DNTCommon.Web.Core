using System;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class CacheServiceController : Controller
{
    private readonly ICacheService _cacheService;
    private const string Key = "Key1";

    public CacheServiceController(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public IActionResult Index()
    {
        var dateTime = _cacheService.GetOrAdd(Key, () => DateTime.Now, DateTimeOffset.UtcNow.AddDays(1));
        return View(dateTime);
    }
}