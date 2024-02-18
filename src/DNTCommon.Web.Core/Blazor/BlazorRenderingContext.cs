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
}