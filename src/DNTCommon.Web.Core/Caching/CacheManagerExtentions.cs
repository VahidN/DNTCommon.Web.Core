﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace DNTCommon.Web.Core;

/// <summary>
///     CacheManager Extentions
/// </summary>
public static class CacheManagerExtentions
{
    private static readonly string[] CacheControlValues = ["no-cache", "max-age=0", "must-revalidate", "no-store"];

    /// <summary>
    ///     Sets `no-cache`, `must-revalidate`, `no-store` headers for the current `Response`.
    /// </summary>
    public static void DisableBrowserCache(this HttpContext httpContext)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        // Note: https://docs.microsoft.com/en-us/aspnet/core/performance/caching/middleware
        // The Antiforgery system for generating secure tokens to prevent Cross-Site Request Forgery (CSRF)
        // attacks sets the Cache-Control and Pragma headers to no-cache so that responses aren't cached.
        // More info:
        // https://github.com/aspnet/Antiforgery/blob/dev/src/Microsoft.AspNetCore.Antiforgery/Internal/DefaultAntiforgery.cs#L381
        // https://github.com/aspnet/Antiforgery/issues/116
        // https://github.com/aspnet/Security/issues/1474
        // So ... the following settings won't work for the pages with normal forms with default settings.
        httpContext.Response.Headers[HeaderNames.CacheControl] = new StringValues(CacheControlValues);
        httpContext.Response.Headers[HeaderNames.Expires] = "-1";
        httpContext.Response.Headers[HeaderNames.Pragma] = "no-cache";
    }
}