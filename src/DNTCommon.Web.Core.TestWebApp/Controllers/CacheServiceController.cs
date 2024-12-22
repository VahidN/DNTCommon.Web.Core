using System;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class CacheServiceController : Controller
{
    private const string Key = "Key1";
    private readonly ICacheService _cacheService;

    public CacheServiceController(ICacheService cacheService) => _cacheService = cacheService;

    public IActionResult Index()
    {
        var dateTime = _cacheService.GetOrAdd(Key, nameof(CacheServiceController), () => DateTime.Now,
            DateTimeOffset.UtcNow.AddDays(days: 1));

        return View(dateTime);
    }
}