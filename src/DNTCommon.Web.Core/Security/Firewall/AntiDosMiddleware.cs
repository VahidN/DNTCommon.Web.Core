using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

/// <summary>
/// AntiDos Middleware
/// </summary>
public class AntiDosMiddleware
{
    private readonly RequestDelegate _next;
    private IOptionsSnapshot<AntiDosConfig>? _antiDosConfig;

    /// <summary>
    /// AntiDos Middleware
    /// </summary>
    public AntiDosMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// AntiDos Middleware Pipeline
    /// </summary>
    public async Task Invoke(
         HttpContext context,
         IOptionsSnapshot<AntiDosConfig> antiDosConfig,
         IAntiDosFirewall antiDosFirewall)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (antiDosFirewall == null)
        {
            throw new ArgumentNullException(nameof(antiDosFirewall));
        }

        _antiDosConfig = antiDosConfig ?? throw new ArgumentNullException(nameof(antiDosConfig));
        if (_antiDosConfig.Value == null)
        {
            throw new ArgumentNullException(nameof(antiDosConfig), "Please add AntiDosConfig to your appsettings.json file.");
        }

        var requestInfo = getHeadersInfo(context);

        var validationResult = antiDosFirewall.ShouldBlockClient(requestInfo);
        if (validationResult.ShouldBlockClient)
        {
            antiDosFirewall.LogIt(validationResult.ThrottleInfo, requestInfo);
            addResetHeaders(context, validationResult.ThrottleInfo);
            await blockClient(context);
            return;
        }
        await _next(context);
    }

    private static AntiDosFirewallRequestInfo getHeadersInfo(HttpContext context)
    {
        return new AntiDosFirewallRequestInfo(context.Request.Headers)
        {
            IP = context.GetIP(),
            UserAgent = context.GetUserAgent(),
            UrlReferrer = context.GetReferrerUri(),
            RawUrl = context.GetRawUrl(),
            IsLocal = context.IsLocal()
        };
    }

    private Task blockClient(HttpContext context)
    {
        // see 409 - http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
        context.Response.StatusCode = (int)HttpStatusCode.Conflict;
        return context.Response.WriteAsync(_antiDosConfig?.Value?.ErrorMessage ?? "The server is busy!");
    }

    private void addResetHeaders(HttpContext context, ThrottleInfo? throttleInfo)
    {
        if (throttleInfo == null || _antiDosConfig == null)
        {
            return;
        }
        context.Response.Headers["X-RateLimit-Limit"] = _antiDosConfig.Value.AllowedRequests.ToString(CultureInfo.InvariantCulture);
        var requestsRemaining = Math.Max(_antiDosConfig.Value.AllowedRequests - throttleInfo.RequestsCount, 0);
        context.Response.Headers["X-RateLimit-Remaining"] = requestsRemaining.ToString(CultureInfo.InvariantCulture);
        context.Response.Headers["X-RateLimit-Reset"] = throttleInfo.ExpiresAt.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        context.Response.Headers["Retry-After"] = context.Response.Headers["X-RateLimit-Reset"];
    }
}