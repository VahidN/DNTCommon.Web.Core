using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace DNTCommon.Web.Core;

/// <summary>
///     Determines the current rendering environment of a Blazor page.
///     Inspired from https://github.com/dotnet/aspnetcore/issues/49401
/// </summary>
public class BlazorRenderingContext(IHttpContextAccessor httpContextAccessor, IJSRuntime jsRuntime)
    : IBlazorRenderingContext
{
    /// <summary>
    ///     Determines the current rendering environment of a Blazor page.
    /// </summary>
    public BlazorRenderingEnvironment GetCurrentRenderingEnvironment()
    {
        if (OperatingSystem.IsBrowser())
        {
            return BlazorRenderingEnvironment.Wasm;
        }

        if (httpContextAccessor.HttpContext is null)
        {
            return BlazorRenderingEnvironment.Console;
        }

        if (!httpContextAccessor.HttpContext.Response.HasStarted)
        {
            return BlazorRenderingEnvironment.SsrPrerendering;
        }

        var jsRuntimeName = jsRuntime.GetType().Name;

        if (string.Equals(jsRuntimeName, "RemoteJSRuntime", StringComparison.Ordinal))
        {
            return BlazorRenderingEnvironment.InteractiveServer;
        }

        if (string.Equals(jsRuntimeName, "WebViewJSRuntime", StringComparison.Ordinal))
        {
            return BlazorRenderingEnvironment.WebView;
        }

        return BlazorRenderingEnvironment.Unknown;
    }

    /// <summary>
    ///     Is current request a Blazor Enhanced Navigation
    /// </summary>
    public bool IsBlazorEnhancedNavigation
    {
        get
        {
            var request = httpContextAccessor.HttpContext?.Request;

            return request is not null && (request.Headers.ContainsKey("blazor-enhanced-nav") ||
                                           request.Headers.ContainsKey("Sec-Fetch-Mode") ||
                                           request.Headers.Values.Contains("text/html; blazor-enhanced-nav=on"));
        }
    }
}