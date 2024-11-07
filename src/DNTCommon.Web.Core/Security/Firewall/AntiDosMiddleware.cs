using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

/// <summary>
///     AntiDos Middleware
/// </summary>
public class AntiDosMiddleware
{
    private readonly IAntiDosFirewall _antiDosFirewall;
    private readonly RequestDelegate _next;
    private AntiDosConfig _antiDosConfig;

    /// <summary>
    ///     AntiDos Middleware
    /// </summary>
    public AntiDosMiddleware(RequestDelegate next,
        IOptionsMonitor<AntiDosConfig> antiDosConfig,
        IAntiDosFirewall antiDosFirewall)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(antiDosFirewall);
        ArgumentNullException.ThrowIfNull(antiDosConfig);

        _next = next;
        _antiDosFirewall = antiDosFirewall;
        _antiDosConfig = antiDosConfig.CurrentValue;

        if (_antiDosConfig == null)
        {
            throw new ArgumentNullException(nameof(antiDosConfig),
                message: "Please add AntiDosConfig to your appsettings.json file.");
        }

        antiDosConfig.OnChange(options => { _antiDosConfig = options; });
    }

    /// <summary>
    ///     AntiDos Middleware Pipeline
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!_antiDosConfig.Disable)
        {
            var requestInfo = GetHeadersInfo(context);
            var validationResult = _antiDosFirewall.ShouldBlockClient(requestInfo);

            if (validationResult.ShouldBlockClient)
            {
                _antiDosFirewall.LogIt(validationResult.ThrottleInfo, requestInfo);
                AddResetHeaders(context, validationResult.ThrottleInfo);
                await BlockClient(context);

                return;
            }
        }

        await _next(context);
    }

    private static AntiDosFirewallRequestInfo GetHeadersInfo(HttpContext context)
        => new(context.Request.Headers)
        {
            IP = context.GetIP(),
            UserAgent = context.GetUserAgent(),
            UrlReferrer = context.GetReferrerUri(),
            RawUrl = context.GetRawUrl(),
            IsLocal = context.IsLocal()
        };

    private Task BlockClient(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;

        return context.Response.WriteAsync(_antiDosConfig?.ErrorMessage ?? "The server is busy!");
    }

    private void AddResetHeaders(HttpContext context, ThrottleInfo? throttleInfo)
    {
        if (throttleInfo == null || _antiDosConfig == null)
        {
            return;
        }

        context.Response.Headers[key: "X-RateLimit-Limit"] =
            _antiDosConfig.AllowedRequests.ToString(CultureInfo.InvariantCulture);

        var requestsRemaining = Math.Max(_antiDosConfig.AllowedRequests - throttleInfo.RequestsCount, val2: 0);

        context.Response.Headers[key: "X-RateLimit-Remaining"] =
            requestsRemaining.ToString(CultureInfo.InvariantCulture);

        context.Response.Headers[key: "X-RateLimit-Reset"] =
            throttleInfo.ExpiresAt.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);

        context.Response.Headers[key: "Retry-After"] = context.Response.Headers[key: "X-RateLimit-Reset"];
    }
}