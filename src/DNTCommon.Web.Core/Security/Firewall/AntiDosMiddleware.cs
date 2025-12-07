using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

/// <summary>
///     AntiDos Middleware
/// </summary>
public sealed class AntiDosMiddleware : IDisposable
{
    private readonly IAntiDosFirewall _antiDosFirewall;
    private readonly IDisposable? _disposableOnChange;
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

        if (_antiDosConfig is null)
        {
            throw new ArgumentNullException(nameof(antiDosConfig),
                message: "Please add AntiDosConfig to your appsettings.json file.");
        }

        _disposableOnChange = antiDosConfig.OnChange(options => { _antiDosConfig = options; });
    }

    public void Dispose() => _disposableOnChange?.Dispose();

#pragma warning disable CC001,MA0137
    /// <summary>
    ///     AntiDos Middleware Pipeline
    /// </summary>
    public async Task Invoke(HttpContext context)
#pragma warning restore CC001, MA0137
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
                await BlockClientAsync(context);

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
            IsLocal = context.IsLocal(),
            IsBot = context.IsBot()
        };

    private Task BlockClientAsync(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;

        return context.Response.WriteAsync(_antiDosConfig?.ErrorMessage ?? "The server is busy!");
    }

    private void AddResetHeaders(HttpContext context, ThrottleInfo? throttleInfo)
    {
        if (throttleInfo is null || _antiDosConfig is null)
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

        context.Response.Headers.RetryAfter = context.Response.Headers[key: "X-RateLimit-Reset"];
    }
}
