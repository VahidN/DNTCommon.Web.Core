using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     RedirectManager Extensions
/// </summary>
public static class BlazorSsrRedirectManagerExtensions
{
    private const string StatusCookieName = "Identity.StatusMessage";

    private static readonly CookieBuilder StatusCookieBuilder = new()
    {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        IsEssential = true,
        MaxAge = TimeSpan.FromSeconds(5)
    };

    /// <summary>
    ///     Returns a value that indicates if the HTTP request method is GET.
    /// </summary>
    public static bool IsGetRequest(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        return HttpMethods.IsGet(httpContext.Request.Method);
    }

    /// <summary>
    ///     Returns a value that indicates if the HTTP request method is POST
    /// </summary>
    public static bool IsPostRequest(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        return HttpMethods.IsPost(httpContext.Request.Method);
    }

    /// <summary>
    ///     It's used to redirect to the current page
    /// </summary>
    public static string GetCurrentPath(this NavigationManager navigationManager)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);

        return navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);
    }

    /// <summary>
    ///     Prevents open redirect attacks
    /// </summary>
    public static string GetSafeRedirectUri(this NavigationManager navigationManager, string? uri)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);
        uri ??= "/";

        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = navigationManager.ToBaseRelativePath(uri);
        }

        return uri;
    }

    /// <summary>
    ///     An exception-less redirect solution for the Blazor SSR! It requires the enhanced navigation enabled to work.
    /// </summary>
    public static void SsrRedirectTo(this HttpContext httpContext, string redirectionUrl, int statusCode = 200)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Response.Headers.Append("blazor-enhanced-nav-redirect-location", redirectionUrl);
        httpContext.Response.StatusCode = statusCode;
    }

    /// <summary>
    ///     Redirects the current Blazor's SSR page to another page with specified params
    ///     It requires the enhanced navigation enabled to work.
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="navigationManager"></param>
    /// <param name="uri"></param>
    /// <param name="queryParameters"></param>
    public static void SsrRedirectTo(this HttpContext httpContext,
        NavigationManager navigationManager,
        string uri,
        IReadOnlyDictionary<string, object?> queryParameters)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(navigationManager);
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        httpContext.SsrRedirectTo(newUri);
    }

    /// <summary>
    ///     Redirects the current Blazor's SSR page to another page
    ///     This method will never return under any circumstance.
    /// </summary>
    /// <param name="navigationManager"></param>
    /// <param name="uri"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [DoesNotReturn]
    public static void SsrRedirectTo(this NavigationManager navigationManager, string? uri)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);

        uri = navigationManager.GetSafeRedirectUri(uri);

        //  When you tell the navigation manager to navigate, the system needs to know to stop the whole prerendering process that is taking place.
        // This is a normal part of the redirect-during-prerendering behavior.
        // During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
        // So as long as this is called from a statically rendered Identity component, the InvalidOperationException is never thrown.
        navigationManager.NavigateTo(uri);

        // https://github.com/dotnet/aspnetcore/issues/53996
        // https://github.com/dotnet/aspnetcore/issues/13582#issuecomment-527383363

        throw new InvalidOperationException("This method can only be used during static rendering.");
    }

    /// <summary>
    ///     Redirects the current Blazor's SSR page to another page with specified params
    ///     This method will never return under any circumstance.
    /// </summary>
    /// <param name="navigationManager"></param>
    /// <param name="uri"></param>
    /// <param name="queryParameters"></param>
    [DoesNotReturn]
    public static void SsrRedirectTo(this NavigationManager navigationManager,
        string uri,
        IReadOnlyDictionary<string, object?> queryParameters)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        navigationManager.SsrRedirectTo(newUri);
    }

    /// <summary>
    ///     Redirects the current Blazor's SSR page to another page with the StatusCookieBuilder
    ///     This method will never return under any circumstance.
    /// </summary>
    /// <param name="navigationManager"></param>
    /// <param name="uri"></param>
    /// <param name="message"></param>
    /// <param name="context"></param>
    [DoesNotReturn]
    public static void SsrRedirectToWithStatus(this NavigationManager navigationManager,
        string uri,
        string message,
        HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));
        navigationManager.SsrRedirectTo(uri);
    }

    /// <summary>
    ///     Redirects the current Blazor's SSR page to the same page.
    ///     This method will never return under any circumstance.
    /// </summary>
    /// <param name="navigationManager"></param>
    [DoesNotReturn]
    public static void SsrRedirectToCurrentPage(this NavigationManager navigationManager)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);
        navigationManager.SsrRedirectTo(navigationManager.GetCurrentPath());
    }

    /// <summary>
    ///     Redirects the current Blazor's SSR page to the same page with the StatusCookieBuilder
    ///     This method will never return under any circumstance.
    /// </summary>
    /// <param name="navigationManager"></param>
    /// <param name="message"></param>
    /// <param name="context"></param>
    [DoesNotReturn]
    public static void SsrRedirectToCurrentPageWithStatus(this NavigationManager navigationManager,
        string message,
        HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);
        navigationManager.SsrRedirectToWithStatus(navigationManager.GetCurrentPath(), message, context);
    }

    /// <summary>
    ///     Returns the ToBaseRelativePath of current page
    /// </summary>
    public static string GetCurrentRelativePath(this NavigationManager navigationManager)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);

        return navigationManager.ToBaseRelativePath(navigationManager.Uri).NormalizeRelativePath();
    }

    /// <summary>
    ///     Removes `/` and `#` from the URL
    /// </summary>
    public static string NormalizeRelativePath(this string? path)
    {
        path = path?.TrimStart('/', '#');

        if (string.IsNullOrWhiteSpace(path))
        {
            path = "/";
        }

        return path.Trim();
    }
}