using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DNTCommon.Web.Core;

/// <summary>
///     Http Request Info
/// </summary>
public class HttpRequestInfoService(IHttpContextAccessor httpContextAccessor, IUrlHelper urlHelper)
    : IHttpRequestInfoService
{
    /// <summary>
    ///     Gets the current HttpContext.Request's UserAgent.
    /// </summary>
    public string? GetUserAgent() => GetHeaderValue(HeaderNames.UserAgent);

    /// <summary>
    ///     Gets the current HttpContext.Request's Referrer.
    /// </summary>
    public string? GetReferrerUrl() => httpContextAccessor.HttpContext?.GetReferrerUrl();

    /// <summary>
    ///     Gets the current HttpContext.Request's Referrer.
    /// </summary>
    public Uri? GetReferrerUri() => httpContextAccessor.HttpContext?.GetReferrerUri();

    /// <summary>
    ///     Gets the current HttpContext.Request's IP.
    /// </summary>
    public string? GetIP(bool tryUseXForwardHeader = true)
        => httpContextAccessor.HttpContext?.GetIP(tryUseXForwardHeader);

    /// <summary>
    ///     Gets a current HttpContext.Request's header value.
    /// </summary>
    public string? GetHeaderValue(string headerName) => httpContextAccessor.HttpContext?.GetHeaderValue(headerName);

    /// <summary>
    ///     Gets the current HttpContext.Request content's absolute path.
    ///     If the specified content path does not start with the tilde (~) character, this method returns contentPath
    ///     unchanged.
    /// </summary>
    public Uri? AbsoluteContent(string contentPath)
    {
        var baseUri = GetBaseUri();

        return baseUri == null ? null : new Uri(baseUri, urlHelper.Content(contentPath));
    }

    /// <summary>
    ///     Gets the current HttpContext.Request's root address.
    /// </summary>
    public Uri? GetBaseUri()
    {
        var uriString = GetBaseUrl();

        return uriString == null ? null : new Uri(uriString);
    }

    /// <summary>
    ///     Gets the current HttpContext.Request's root address.
    /// </summary>
    public string? GetBaseUrl() => httpContextAccessor.HttpContext?.GetBaseUrl();

    /// <summary>
    ///     Gets the current HttpContext.Request's address.
    /// </summary>
    public string? GetRawUrl() => httpContextAccessor.HttpContext?.GetRawUrl();

    /// <summary>
    ///     Gets the current HttpContext.Request's address.
    /// </summary>
    public Uri? GetRawUri()
    {
        var uriString = GetRawUrl();

        return uriString == null ? null : new Uri(uriString);
    }

    /// <summary>
    ///     Gets the current HttpContext.Request's IUrlHelper.
    /// </summary>
    public IUrlHelper GetUrlHelper() => urlHelper;

    /// <summary>
    ///     Deserialize `request.Body` as a JSON content.
    /// </summary>
    public Task<T?> DeserializeRequestJsonBodyAsAsync<T>()
    {
        if (httpContextAccessor.HttpContext == null)
        {
            return Task.FromResult(default(T));
        }

        return httpContextAccessor.HttpContext.DeserializeRequestJsonBodyAsAsync<T>();
    }

    /// <summary>
    ///     Reads `request.Body` as string.
    /// </summary>
    public Task<string> ReadRequestBodyAsStringAsync()
    {
        if (httpContextAccessor.HttpContext == null)
        {
            return Task.FromResult(string.Empty);
        }

        return httpContextAccessor.HttpContext.ReadRequestBodyAsStringAsync();
    }

    /// <summary>
    ///     Deserialize `request.Body` as a JSON content.
    /// </summary>
    public Task<IDictionary<string, string>?> DeserializeRequestJsonBodyAsDictionaryAsync()
    {
        if (httpContextAccessor.HttpContext == null)
        {
            return Task.FromResult<IDictionary<string, string>?>(result: null);
        }

        return httpContextAccessor.HttpContext.DeserializeRequestJsonBodyAsDictionaryAsync();
    }
}