using System.Text;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace DNTCommon.Web.Core;

/// <summary>
///     Domain Helper Extensions
/// </summary>
public static class DomainHelperExtensions
{
    private static readonly Lazy<List<string>> TldsBuilder = new(DefaultTlds,
        LazyThreadSafetyMode.ExecutionAndPublication);

    private static readonly string[] FeedKeys = ["utm_source", "utm_medium", "utm_campaign", "utm_updated", "updated"];

    /// <summary>
    ///     Tld Patterns
    /// </summary>
    public static IReadOnlyCollection<string> Tlds => TldsBuilder.Value;

    /// <summary>
    ///     Determines whether uri1 and uri2 have the same domain.
    /// </summary>
    public static bool HaveTheSameDomain(this Uri? uri1, Uri? uri2)
    {
        var domain2 = uri2.GetUrlDomain().Domain;
        var domain1 = uri1.GetUrlDomain().Domain;

        return domain2.Equals(domain1, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Determines whether uri1 and uri2 have the same domain.
    /// </summary>
    public static bool HaveTheSameDomain(this string? uri1, string? uri2)
        => uri1.IsValidUrl() && uri2.IsValidUrl() && HaveTheSameDomain(new Uri(uri1), new Uri(uri2));

    /// <summary>
    ///     Determines whether uri1 and uri2 have the same domain.
    /// </summary>
    public static bool HaveTheSameDomain(this string? uri1, Uri? uri2)
        => uri1.IsValidUrl() && HaveTheSameDomain(new Uri(uri1), uri2);

    /// <summary>
    ///     Determines whether the url has no extension.
    /// </summary>
    public static bool IsMvcPage(this Uri? url) => string.IsNullOrWhiteSpace(url.GetUriExtension());

    /// <summary>
    ///     Determines whether the url has no extension.
    /// </summary>
    public static bool IsMvcPage(this string? url) => url.IsValidUrl() && IsMvcPage(new Uri(url));

    /// <summary>
    ///     Determines whether the url has an extension.
    /// </summary>
    public static bool IsStaticFileUrl(this Uri? url) => !string.IsNullOrWhiteSpace(url.GetUriExtension());

    /// <summary>
    ///     Determines whether the url has an extension.
    /// </summary>
    public static bool IsStaticFileUrl(this string? url) => url.IsValidUrl() && IsStaticFileUrl(new Uri(url));

    /// <summary>
    ///     Returns the extension of the uri.
    /// </summary>
    public static string? GetUriExtension(this Uri? uri, bool throwOnException = false)
    {
        try
        {
            return uri == null ? null : Path.GetExtension(uri.PathAndQuery);
        }
        catch
        {
            if (throwOnException)
            {
                throw;
            }

            return null;
        }
    }

    /// <summary>
    ///     Returns the extension of the uri.
    /// </summary>
    public static string? GetUriExtension(this string? uri) => !uri.IsValidUrl() ? null : GetUriExtension(new Uri(uri));

    /// <summary>
    ///     Returns false when hostUri hase the same domain as url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="hostUri"></param>
    /// <returns></returns>
    public static bool IsRemoteUrl([NotNullWhen(returnValue: true)] this string? url, Uri hostUri)
        => url.IsValidUrl() && !hostUri.HaveTheSameDomain(new Uri(url));

    /// <summary>
    ///     Returns the SubDomain of the uri.
    /// </summary>
    public static string? GetSubDomain(this Uri? url)
    {
        if (url == null)
        {
            return null;
        }

        if (url.HostNameType != UriHostNameType.Dns)
        {
            return null;
        }

        var host = url.Host.TrimEnd(trimChar: '.');

        if (host.Split(separator: '.').Length <= 2)
        {
            return null;
        }

        var lastIndex = host.LastIndexOf(value: '.');
        var index = host.LastIndexOf(value: '.', lastIndex - 1);

        return host[..index];
    }

    /// <summary>
    ///     Returns the SubDomain of the uri.
    /// </summary>
    public static string? GetSubDomain(this string? url) => url.IsValidUrl() ? GetSubDomain(new Uri(url)) : null;

    /// <summary>
    ///     Returns the host part without its SubDomain.
    /// </summary>
    public static string? GetHostWithoutSubDomain(this Uri? url)
    {
        if (url is null)
        {
            return null;
        }

        var subdomain = GetSubDomain(url);
        var host = url.Host.TrimEnd(trimChar: '.');

        if (subdomain != null)
        {
            host = host.Replace(string.Format(CultureInfo.InvariantCulture, format: "{0}.", subdomain), string.Empty,
                StringComparison.OrdinalIgnoreCase);
        }

        return host;
    }

    /// <summary>
    ///     Returns the host part without its SubDomain.
    /// </summary>
    public static string? GetHostWithoutSubDomain(this string? url)
        => !url.IsValidUrl() ? null : GetHostWithoutSubDomain(new Uri(url));

    /// <summary>
    ///     Returns the domain part of the url.
    /// </summary>
    public static (string Domain, bool HasBestMatch) GetUrlDomain(this string? url)
        => url is null ? (string.Empty, false) : GetUrlDomain(new Uri(url));

    /// <summary>
    ///     Returns the domain part of the url.
    /// </summary>
    public static (string Domain, bool HasBestMatch) GetUrlDomain(this Uri? url)
    {
        if (url == null)
        {
            return (string.Empty, false);
        }

        var dotBits = url.Host.Split(separator: '.');

        switch (dotBits.Length)
        {
            case 1:
            case 2:
                return (url.Host, false); //eg http://localhost/blah.php = "localhost"
        }

        var bestMatch = "";

        foreach (var tld in Tlds)
        {
            if (url.Host.EndsWith(tld, StringComparison.OrdinalIgnoreCase) && tld.Length > bestMatch.Length)
            {
                bestMatch = tld;
            }
        }

        if (string.IsNullOrEmpty(bestMatch))
        {
            return (url.Host, false); //eg http://domain.com/blah = "domain.com"
        }

        //add the domain name onto tld
        var bestBits = bestMatch.Split(separator: '.');
        var inputBits = url.Host.Split(separator: '.');
        var getLastBits = bestBits.Length + 1;
        var bestMatchBuilder = new StringBuilder();

        for (var c = inputBits.Length - getLastBits; c < inputBits.Length; c++)
        {
            if (bestMatchBuilder.Length > 0)
            {
                bestMatchBuilder.Append(value: '.');
            }

            bestMatchBuilder.Append(inputBits[c]);
        }

        return (bestMatchBuilder.ToString(), true);
    }

    /// <summary>
    ///     Determines whether the `referrer` has the same host or domain as `url`.
    /// </summary>
    public static bool IsLocalReferrer(this Uri? referrer, Uri? url)
        => (referrer is not null && url is not null && referrer.Host.TrimEnd(trimChar: '.')
               .Equals(url.Host.TrimEnd(trimChar: '.'), StringComparison.OrdinalIgnoreCase)) ||
           HaveTheSameDomain(referrer, url);

    /// <summary>
    ///     Determines whether the `referrer` has the same host or domain as `url`.
    /// </summary>
    public static bool IsLocalReferrer(this string? referrer, string? url)
        => referrer.IsValidUrl() && url.IsValidUrl() && IsLocalReferrer(new Uri(referrer), new Uri(url));

    /// <summary>
    ///     Determines whether the `destUri` has the same domain as `siteRootUrl`.
    /// </summary>
    public static bool IsReferrerToThisSite(this Uri? destUri, string siteRootUrl)
    {
        if (destUri == null || string.IsNullOrWhiteSpace(siteRootUrl))
        {
            return false;
        }

        var siteDomain = siteRootUrl.GetUrlDomain().Domain;
        var destDomain = GetUrlDomain(destUri).Domain;

        return destDomain.Equals(siteDomain, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Determines whether the `destUri` has the same domain as `siteRootUrl`.
    /// </summary>
    public static bool IsReferrerToThisSite(this string destUri, string siteRootUrl)
        => IsReferrerToThisSite(new Uri(destUri), siteRootUrl);

    private static List<string> DefaultTlds()
    {
        var tlds = new List<string>();
        tlds.AddRange(TldPatterns.EXACT);
        tlds.AddRange(TldPatterns.UNDER);
        tlds.AddRange(TldPatterns.EXCLUDED);

        return tlds;
    }

    /// <summary>
    ///     Checks for "utm_source", "utm_medium", "utm_campaign", "utm_updated", "updated" in the Url
    /// </summary>
    /// <param name="navigationManager"></param>
    /// <returns></returns>
    public static bool IsFromFeed(this NavigationManager? navigationManager)
    {
        if (navigationManager is null)
        {
            return false;
        }

        var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);

        return IsFromFeed(uri);
    }

    /// <summary>
    ///     Checks for "utm_source", "utm_medium", "utm_campaign", "utm_updated", "updated" in the Url
    /// </summary>
    public static bool IsFromFeed(this Uri? uri)
    {
        if (uri is null)
        {
            return false;
        }

        var keyValues = QueryHelpers.ParseQuery(uri.Query);

        return FeedKeys.Any(keyValues.ContainsKey);
    }

    /// <summary>
    ///     Removes the "utm_source", "utm_medium", "utm_campaign", "utm_updated", "updated" keys from the query strings of the
    ///     url
    /// </summary>
    public static string? GetUrlWithoutRssQueryStrings(this string? url) => url.GetUrlWithoutQueryStrings(FeedKeys);

    /// <summary>
    ///     Removes the given keys from the query strings of the url
    /// </summary>
    public static string? GetUrlWithoutQueryStrings(this string? url, params string[]? keys)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        if (!url.IsValidUrl())
        {
            return null;
        }

        var uri = new Uri(url);

        if (string.IsNullOrWhiteSpace(uri.Query))
        {
            return url;
        }

        var queryStrings = HttpUtility.ParseQueryString(uri.Query);

        if (keys is not null)
        {
            foreach (var key in keys)
            {
                queryStrings.Remove(key);
            }
        }

        var pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

        return queryStrings.Count > 0
            ? string.Create(CultureInfo.InvariantCulture, $"{pagePathWithoutQueryString}?{queryStrings}")
            : pagePathWithoutQueryString;
    }

    /// <summary>
    ///     Path.Combine for URLs. The `relativeUrl` doesn't necessarily need a `/` at the beginning of it.
    /// </summary>
    public static string CombineUrl(this string? baseUrl, string? relativeUrl, bool escapeRelativeUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = "/";
        }

        baseUrl = baseUrl.Trim().Replace(oldValue: "\\", newValue: "/", StringComparison.Ordinal);

        if (string.IsNullOrWhiteSpace(relativeUrl))
        {
            return baseUrl;
        }

        baseUrl = baseUrl.TrimEnd(trimChar: '/');

        relativeUrl = relativeUrl.Trim()
            .Replace(oldValue: "\\", newValue: "/", StringComparison.Ordinal)
            .TrimStart(trimChar: '/');

        return escapeRelativeUrl ? $"{baseUrl}/{Uri.EscapeDataString(relativeUrl)}" : $"{baseUrl}/{relativeUrl}";
    }

    /// <summary>
    ///     Path.Combine for URLs. The `relativeUrl` doesn't necessarily need a `/` at the beginning of it.
    /// </summary>
    public static string? CombineUrl([NotNullIfNotNull(nameof(baseUri))] this Uri? baseUri,
        string? relativeUrl,
        bool escapeRelativeUrl)
        => baseUri?.AbsoluteUri.CombineUrl(relativeUrl, escapeRelativeUrl);

    /// <summary>
    ///     Path.Combine for URLs. The `relativeUrls` don't necessarily need a `/` at the beginning of them.
    /// </summary>
    public static string? CombineUrls([NotNullIfNotNull(nameof(baseUri))] this Uri? baseUri,
        bool escapeRelativeUrl,
        params string[]? relativePaths)
        => baseUri?.AbsoluteUri.CombineUrls(escapeRelativeUrl, relativePaths);

    /// <summary>
    ///     Path.Combine for URLs. The `relativeUrls` don't necessarily need a `/` at the beginning of them.
    /// </summary>
    public static string CombineUrls(this string? baseUrl, bool escapeRelativeUrl, params string[]? relativePaths)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = "/";
        }

        if (relativePaths is null || relativePaths.Length == 0)
        {
            return baseUrl;
        }

        var currentUrl = baseUrl.CombineUrl(relativePaths[0], escapeRelativeUrl);

        return currentUrl.CombineUrls(escapeRelativeUrl, relativePaths.Skip(count: 1).ToArray());
    }
}