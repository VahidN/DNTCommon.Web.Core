using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace DNTCommon.Web.Core;

/// <summary>
///     Http Request Extensions
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    ///     Gets the current HttpContext.Request's UserAgent.
    /// </summary>
    public static string GetUserAgent(this HttpContext httpContext)
        => httpContext.GetHeaderValue(HeaderNames.UserAgent);

    /// <summary>
    ///     Does this route has an AuthorizeAttribute?
    /// </summary>
    public static bool IsProtectedRoute(this HttpContext? context)
        => context?.GetEndpoint()?.Metadata?.GetMetadata<AuthorizeAttribute>() is not null;

    /// <summary>
    ///     Gets the current HttpContext.Request's Referrer.
    /// </summary>
    public static string GetReferrerUrl(this HttpContext httpContext)
        => httpContext.GetHeaderValue(HeaderNames.Referer); // The HTTP referer (originally a misspelling of referrer)

    /// <summary>
    ///     Gets the current HttpContext.Request's Referrer.
    /// </summary>
    public static Uri? GetReferrerUri(this HttpContext httpContext)
    {
        var referrer = httpContext.GetReferrerUrl();

        if (string.IsNullOrWhiteSpace(referrer))
        {
            return null;
        }

        return Uri.TryCreate(referrer, UriKind.Absolute, out var result) ? result : null;
    }

    /// <summary>
    ///     Gets the current HttpContext.Request's IP.
    /// </summary>
    public static string? GetIP(this HttpContext httpContext, bool tryUseXForwardHeader = true)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        var ip = string.Empty;

        // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

        // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
        // for 99% of cases however it has been suggested that a better (although tedious)
        // approach might be to read each IP from right to left and use the first public static IP.
        // http://stackoverflow.com/a/43554000/538763
        //
        if (tryUseXForwardHeader)
        {
            ip = SplitCsv(httpContext.GetHeaderValue(headerName: "X-Forwarded-For")).FirstOrDefault();
        }

        // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
        if (string.IsNullOrWhiteSpace(ip) && httpContext.Connection?.RemoteIpAddress != null)
        {
            ip = httpContext.Connection.RemoteIpAddress.ToString();
        }

        if (string.IsNullOrWhiteSpace(ip))
        {
            ip = httpContext.GetHeaderValue(headerName: "REMOTE_ADDR");
        }

        return ip;
    }

    /// <summary>
    ///     Gets a current HttpContext.Request's header value.
    /// </summary>
    public static string GetHeaderValue(this HttpContext httpContext, string headerName)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        StringValues values = string.Empty;

        if (httpContext.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
        {
            return values.ToString();
        }

        return string.Empty;
    }

    private static List<string> SplitCsv(string csvList)
    {
        if (string.IsNullOrWhiteSpace(csvList))
        {
            return new List<string>();
        }

        return csvList.TrimEnd(trimChar: ',').Split(separator: ',').AsEnumerable().Select(s => s.Trim()).ToList();
    }

    /// <summary>
    ///     Gets the current HttpContext.Request content's absolute path.
    ///     If the specified content path does not start with the tilde (~) character, this method returns contentPath
    ///     unchanged.
    /// </summary>
    public static Uri AbsoluteContent(this HttpContext httpContext, string contentPath)
    {
        var urlHelper = httpContext.GetUrlHelper();

        return new Uri(httpContext.GetBaseUri(), urlHelper.Content(contentPath));
    }

    /// <summary>
    ///     Creates the action's URL
    /// </summary>
    public static string? GetActionUrl(this HttpContext httpContext, string action, string controller)
    {
        var urlHelper = httpContext.GetUrlHelper();

        return urlHelper.Action(action, controller);
    }

    /// <summary>
    ///     Creates the action's URL, even outside of the MVC's pipeline
    /// </summary>
    public static string? GetGenericActionUrl(this HttpContext httpContext, string action, string controller)
    {
        var generator = httpContext.GetLinkGenerator();

        return generator.GetPathByAction(action, controller);
    }

    /// <summary>
    ///     Gets the current HttpContext.Request's root address.
    /// </summary>
    public static Uri GetBaseUri(this HttpContext httpContext) => new(httpContext.GetBaseUrl());

    /// <summary>
    ///     Gets the current HttpContext.Request's root address.
    /// </summary>
    public static string GetBaseUrl(this HttpContext httpContext)
    {
        requestSanityCheck(httpContext);
        var request = httpContext.Request;

        return $"{request.Scheme}://{request.Host.ToUriComponent()}";
    }

    /// <summary>
    ///     Gets the current HttpContext.Request's address.
    /// </summary>
    public static string GetRawUrl(this HttpContext httpContext)
    {
        requestSanityCheck(httpContext);

        return httpContext.Request.GetDisplayUrl();
    }

    /// <summary>
    ///     Gets the current HttpContext.Request's address.
    /// </summary>
    public static Uri GetRawUri(this HttpContext httpContext) => new(GetRawUrl(httpContext));

    /// <summary>
    ///     Gets the current HttpContext.Request's IUrlHelper.
    /// </summary>
    public static IUrlHelper GetUrlHelper(this HttpContext httpContext)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        return httpContext.RequestServices.GetRequiredService<IUrlHelper>();
    }

    /// <summary>
    ///     Gets the current HttpContext.Request's LinkGenerator.
    /// </summary>
    public static LinkGenerator GetLinkGenerator(this HttpContext httpContext)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        return httpContext.RequestServices.GetRequiredService<LinkGenerator>();
    }

    private static void requestSanityCheck(this HttpContext httpContext)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        if (httpContext.Request == null)
        {
            throw new InvalidOperationException(message: "HttpContext.Request is null.");
        }
    }

    /// <summary>
    ///     Deserialize `request.Body` as a JSON content.
    ///     This method needs [EnableReadableBodyStream] attribute to be added to a given action method.
    /// </summary>
    public static async Task<T?> DeserializeRequestJsonBodyAsAsync<T>(this HttpContext httpContext)
    {
        var body = await httpContext.ReadRequestBodyAsStringAsync();

        return JsonSerializer.Deserialize<T>(body);
    }

    /// <summary>
    ///     Reads `request.Body` as string.
    ///     This method needs [EnableReadableBodyStream] attribute to be added to a given action method.
    /// </summary>
    public static async Task<string> ReadRequestBodyAsStringAsync(this HttpContext httpContext)
    {
        requestSanityCheck(httpContext);
        var request = httpContext.Request;

        if (request.Body.CanSeek)
        {
            request.Body.Position = 0;
        }
        else
        {
            throw new InvalidOperationException(
                message:
                "To read the request stream's body, please apply [EnableReadableBodyStream] attribute to the current action method.");
        }

        using (var bodyReader = new StreamReader(request.Body, Encoding.UTF8))
        {
            var body = await bodyReader.ReadToEndAsync();

            request.Body.Seek(offset: 0,
                SeekOrigin.Begin); // this is required, otherwise model binding will return null

            return body;
        }
    }

    /// <summary>
    ///     Deserialize `request.Body` as a JSON content.
    ///     This method needs [EnableReadableBodyStream] attribute to be added to a given action method.
    /// </summary>
    public static async Task<IDictionary<string, string>?> DeserializeRequestJsonBodyAsDictionaryAsync(
        this HttpContext httpContext)
    {
        var body = await httpContext.ReadRequestBodyAsStringAsync();

        return JsonSerializer.Deserialize<Dictionary<string, string>>(body);
    }

    /// <summary>
    ///     Creates a log message out of the given request
    /// </summary>
    public static string LogRequest(this HttpRequest? httpRequest, int? responseCode)
    {
        var httpContext = httpRequest?.HttpContext;

        if (httpRequest is null || httpContext is null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        sb.AppendLine(HtmlExtensions.CreateHtmlTable(caption: "Request Info", ["Key", "Value"],
        [
            [
                "Response Code",
                !responseCode.HasValue ? "unknown" : responseCode.Value.ToString(CultureInfo.InvariantCulture)
            ],
            ["Trace Identifier", httpContext.TraceIdentifier], ["Protocol", httpRequest.Protocol],
            ["Http Method", httpRequest.Method], ["Url", httpContext.GetCurrentUrl()],
            ["Is Authenticated", httpContext.User.IsAuthenticated().ToString()]
        ]));

        if (httpRequest.Query.Count != 0)
        {
            sb.AppendLine(HtmlExtensions.CreateHtmlTable(caption: "Request Query", ["Key", "Value"],
                httpRequest.Query.Select(header => (List<string>) [header.Key, $"<pre>{header.Value}</pre>"])
                    .ToList()));
        }

        if (httpRequest.Headers.Any())
        {
            sb.AppendLine(HtmlExtensions.CreateHtmlTable(caption: "Request Headers", ["Key", "Value"],
                httpRequest.Headers.Select(header => (List<string>) [header.Key, $"<pre>{header.Value}</pre>"])
                    .ToList()));
        }

        if (httpContext.User.Claims.Any())
        {
            sb.AppendLine(HtmlExtensions.CreateHtmlTable(caption: "User Claims", ["Key", "Value"],
                httpContext.User.Claims.Select(header => (List<string>) [header.Type, $"<pre>{header.Value}</pre>"])
                    .ToList()));
        }

        if (httpRequest is { HasFormContentType: true, Form.Count: > 0 })
        {
            sb.AppendLine(HtmlExtensions.CreateHtmlTable(caption: "User Claims", ["Key", "Value"],
                httpRequest.Form.Select(header => (List<string>) [header.Key, $"<pre>{header.Value}</pre>"]).ToList()));
        }

        var exceptionHandlerFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

        if (exceptionHandlerFeature?.Error is not null)
        {
            var exception = exceptionHandlerFeature.Error.Demystify();

            sb.AppendLine(HtmlExtensions.CreateHtmlTable(caption: "Exception", ["Key", "Value"],
            [
                ["Message", $"<pre>{exception.FormatException()}</pre>"],
                ["StackTrace", $"<pre>{exception.StackTrace}</pre>"]
            ]));
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Returns the current page's addres. It uses IStatusCodeReExecuteFeature for better results.
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static string GetCurrentUrl(this HttpContext? httpContext)
    {
        if (httpContext is null)
        {
            return "";
        }

        var statusCodeReExecuteFeature = httpContext.Features.Get<IStatusCodeReExecuteFeature>();
        var httpRequestPath = statusCodeReExecuteFeature?.OriginalPath ?? httpContext.Request.Path;

        var queryString = statusCodeReExecuteFeature?.OriginalQueryString ??
                          httpContext.Request.QueryString.ToUriComponent();

        return $"{httpRequestPath}{queryString}";
    }
}