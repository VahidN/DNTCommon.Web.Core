using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace DNTCommon.Web.Core;

/// <summary>
/// Http Request Extensions
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Gets the current HttpContext.Request's UserAgent.
    /// </summary>
    public static string GetUserAgent(this HttpContext httpContext)
    {
        return httpContext.GetHeaderValue("User-Agent");
    }

    /// <summary>
    /// Gets the current HttpContext.Request's Referrer.
    /// </summary>
    public static string GetReferrerUrl(this HttpContext httpContext)
    {
        return httpContext.GetHeaderValue("Referer"); // The HTTP referer (originally a misspelling of referrer)
    }

    /// <summary>
    /// Gets the current HttpContext.Request's Referrer.
    /// </summary>
    public static Uri? GetReferrerUri(this HttpContext httpContext)
    {
        var referrer = httpContext.GetReferrerUrl();
        if (string.IsNullOrWhiteSpace(referrer))
        {
            return null;
        }

        return Uri.TryCreate(referrer, UriKind.Absolute, out Uri? result) ? result : null;
    }

    /// <summary>
    /// Gets the current HttpContext.Request's IP.
    /// </summary>
    public static string? GetIP(this HttpContext httpContext, bool tryUseXForwardHeader = true)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        string? ip = string.Empty;

        // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

        // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
        // for 99% of cases however it has been suggested that a better (although tedious)
        // approach might be to read each IP from right to left and use the first public static IP.
        // http://stackoverflow.com/a/43554000/538763
        //
        if (tryUseXForwardHeader)
        {
            ip = SplitCsv(httpContext.GetHeaderValue("X-Forwarded-For")).FirstOrDefault();
        }

        // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
        if (string.IsNullOrWhiteSpace(ip) &&
            httpContext.Connection?.RemoteIpAddress != null)
        {
            ip = httpContext.Connection.RemoteIpAddress.ToString();
        }

        if (string.IsNullOrWhiteSpace(ip))
        {
            ip = httpContext.GetHeaderValue("REMOTE_ADDR");
        }

        return ip;
    }

    /// <summary>
    /// Gets a current HttpContext.Request's header value.
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

        return csvList
            .TrimEnd(',')
            .Split(',')
            .AsEnumerable()
            .Select(s => s.Trim())
            .ToList();
    }

    /// <summary>
    /// Gets the current HttpContext.Request content's absolute path.
    /// If the specified content path does not start with the tilde (~) character, this method returns contentPath unchanged.
    /// </summary>
    public static Uri AbsoluteContent(this HttpContext httpContext, string contentPath)
    {
        var urlHelper = httpContext.GetUrlHelper();
        return new Uri(httpContext.GetBaseUri(), urlHelper.Content(contentPath));
    }

    /// <summary>
    /// Creates the action's URL
    /// </summary>
    public static string? GetActionUrl(this HttpContext httpContext, string action, string controller)
    {
        var urlHelper = httpContext.GetUrlHelper();
        return urlHelper.Action(action, controller);
    }

    /// <summary>
    /// Creates the action's URL, even outside of the MVC's pipeline
    /// </summary>
    public static string? GetGenericActionUrl(this HttpContext httpContext, string action, string controller)
    {
        var generator = httpContext.GetLinkGenerator();
        return generator.GetPathByAction(action, controller);
    }

    /// <summary>
    /// Gets the current HttpContext.Request's root address.
    /// </summary>
    public static Uri GetBaseUri(this HttpContext httpContext)
    {
        return new Uri(httpContext.GetBaseUrl());
    }

    /// <summary>
    /// Gets the current HttpContext.Request's root address.
    /// </summary>
    public static string GetBaseUrl(this HttpContext httpContext)
    {
        requestSanityCheck(httpContext);
        var request = httpContext.Request;
        return $"{request.Scheme}://{request.Host.ToUriComponent()}";
    }

    /// <summary>
    /// Gets the current HttpContext.Request's address.
    /// </summary>
    public static string GetRawUrl(this HttpContext httpContext)
    {
        requestSanityCheck(httpContext);
        return httpContext.Request.GetDisplayUrl();
    }

    /// <summary>
    /// Gets the current HttpContext.Request's address.
    /// </summary>
    public static Uri GetRawUri(this HttpContext httpContext)
    {
        return new Uri(GetRawUrl(httpContext));
    }

    /// <summary>
    /// Gets the current HttpContext.Request's IUrlHelper.
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
    /// Gets the current HttpContext.Request's LinkGenerator.
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
            throw new InvalidOperationException("HttpContext.Request is null.");
        }
    }

    /// <summary>
    /// Deserialize `request.Body` as a JSON content.
    /// This method needs [EnableReadableBodyStream] attribute to be added to a given action method.
    /// </summary>
    public static async Task<T?> DeserializeRequestJsonBodyAsAsync<T>(this HttpContext httpContext)
    {
        var body = await httpContext.ReadRequestBodyAsStringAsync();
        return JsonSerializer.Deserialize<T>(body);
    }

    /// <summary>
    /// Reads `request.Body` as string.
    /// This method needs [EnableReadableBodyStream] attribute to be added to a given action method.
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
            throw new InvalidOperationException("To read the request stream's body, please apply [EnableReadableBodyStream] attribute to the current action method.");
        }

        using (var bodyReader = new StreamReader(request.Body, Encoding.UTF8))
        {
            var body = await bodyReader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin); // this is required, otherwise model binding will return null
            return body;
        }
    }

    /// <summary>
    /// Deserialize `request.Body` as a JSON content.
    /// This method needs [EnableReadableBodyStream] attribute to be added to a given action method.
    /// </summary>
    public static async Task<IDictionary<string, string>?> DeserializeRequestJsonBodyAsDictionaryAsync(this HttpContext httpContext)
    {
        var body = await httpContext.ReadRequestBodyAsStringAsync();
        return JsonSerializer.Deserialize<Dictionary<string, string>>(body);
    }
}