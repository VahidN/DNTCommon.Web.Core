using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Domain Helper Extensions
/// </summary>
public static class DomainHelperExtensions
{
    private static readonly Lazy<List<string>> _tldsBuilder =
        new(defaultTlds, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    ///     Tld Patterns
    /// </summary>
    public static IReadOnlyCollection<string> Tlds => _tldsBuilder.Value;

    /// <summary>
    ///     Determines whether uri1 and uri2 have the same domain.
    /// </summary>
    public static bool HaveTheSameDomain(this Uri uri1, Uri uri2)
    {
        var domain2 = uri2.GetUrlDomain().Domain;
        var domain1 = uri1.GetUrlDomain().Domain;

        return domain2.Equals(domain1, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Determines whether uri1 and uri2 have the same domain.
    /// </summary>
    public static bool HaveTheSameDomain(this string uri1, string uri2)
        => HaveTheSameDomain(new Uri(uri1), new Uri(uri2));

    /// <summary>
    ///     Determines whether the url has no extension.
    /// </summary>
    public static bool IsMvcPage(this Uri url) => string.IsNullOrWhiteSpace(url.GetUriExtension());

    /// <summary>
    ///     Determines whether the url has no extension.
    /// </summary>
    public static bool IsMvcPage(this string url) => IsMvcPage(new Uri(url));

    /// <summary>
    ///     Determines whether the url has an extension.
    /// </summary>
    public static bool IsStaticFileUrl(this Uri url) => !string.IsNullOrWhiteSpace(url.GetUriExtension());

    /// <summary>
    ///     Determines whether the url has an extension.
    /// </summary>
    public static bool IsStaticFileUrl(this string url) => IsStaticFileUrl(new Uri(url));

    /// <summary>
    ///     Returns the extension of the uri.
    /// </summary>
    public static string GetUriExtension(this Uri uri, bool throwOnException = false)
    {
        try
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return Path.GetExtension(uri.PathAndQuery);
        }
        catch
        {
            if (throwOnException)
            {
                throw;
            }

            return string.Empty;
        }
    }

    /// <summary>
    ///     Returns the extension of the uri.
    /// </summary>
    public static string GetUriExtension(this string uri) => GetUriExtension(new Uri(uri));

    /// <summary>
    ///     Returns the SubDomain of the uri.
    /// </summary>
    public static string? GetSubDomain(this Uri url)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
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

        return host.Substring(startIndex: 0, index);
    }

    /// <summary>
    ///     Returns the SubDomain of the uri.
    /// </summary>
    public static string? GetSubDomain(this string url)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        return GetSubDomain(new Uri(url));
    }

    /// <summary>
    ///     Returns the host part without its SubDomain.
    /// </summary>
    public static string GetHostWithoutSubDomain(this Uri url)
    {
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
    public static string GetHostWithoutSubDomain(this string url) => GetHostWithoutSubDomain(new Uri(url));

    /// <summary>
    ///     Returns the domain part of the url.
    /// </summary>
    public static (string Domain, bool HasBestMatch) GetUrlDomain(this string url) => GetUrlDomain(new Uri(url));

    /// <summary>
    ///     Returns the domain part of the url.
    /// </summary>
    public static (string Domain, bool HasBestMatch) GetUrlDomain(this Uri url)
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
    public static bool IsLocalReferrer(this Uri referrer, Uri url)
    {
        if (referrer == null)
        {
            throw new ArgumentNullException(nameof(referrer));
        }

        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        return referrer.Host.TrimEnd(trimChar: '.')
                   .Equals(url.Host.TrimEnd(trimChar: '.'), StringComparison.OrdinalIgnoreCase) ||
               HaveTheSameDomain(referrer, url);
    }

    /// <summary>
    ///     Determines whether the `referrer` has the same host or domain as `url`.
    /// </summary>
    public static bool IsLocalReferrer(this string referrer, string url)
        => IsLocalReferrer(new Uri(referrer), new Uri(url));

    /// <summary>
    ///     Determines whether the `destUri` has the same domain as `siteRootUrl`.
    /// </summary>
    public static bool IsReferrerToThisSite(this Uri destUri, string siteRootUrl)
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

    private static List<string> defaultTlds()
    {
        var tlds = new List<string>();
        tlds.AddRange(TldPatterns.EXACT);
        tlds.AddRange(TldPatterns.UNDER);
        tlds.AddRange(TldPatterns.EXCLUDED);

        return tlds;
    }

    /// <summary>
    ///     Path.Combine for URLs. The `relativeUrl` doesn't necessarily need a `/` at the beginning of it.
    /// </summary>
    public static string CombineUrl(this string baseUrl, string? relativeUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = "/";
        }

        if (string.IsNullOrWhiteSpace(relativeUrl))
        {
            return baseUrl;
        }

        baseUrl = baseUrl.TrimEnd(trimChar: '/');
        relativeUrl = relativeUrl.TrimStart(trimChar: '/');

        return $"{baseUrl}/{relativeUrl}";
    }

    /// <summary>
    ///     Path.Combine for URLs. The `relativeUrl` doesn't necessarily need a `/` at the beginning of it.
    /// </summary>
    public static string CombineUrl(this Uri baseUri, string? relativeUrl)
    {
        ArgumentNullException.ThrowIfNull(baseUri);

        return baseUri.AbsoluteUri.CombineUrl(relativeUrl);
    }

    /// <summary>
    ///     Path.Combine for URLs. The `relativeUrls` don't necessarily need a `/` at the beginning of them.
    /// </summary>
    public static string CombineUrls(this Uri baseUri, params string[]? relativePaths)
    {
        ArgumentNullException.ThrowIfNull(baseUri);

        return baseUri.AbsoluteUri.CombineUrls(relativePaths);
    }

    /// <summary>
    ///     Path.Combine for URLs. The `relativeUrls` don't necessarily need a `/` at the beginning of them.
    /// </summary>
    public static string CombineUrls(this string baseUrl, params string[]? relativePaths)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = "/";
        }

        if (relativePaths is null)
        {
            return baseUrl;
        }

        if (relativePaths.Length == 0)
        {
            return baseUrl;
        }

        var currentUrl = baseUrl.CombineUrl(relativePaths[0]);

        return currentUrl.CombineUrls(relativePaths.Skip(count: 1).ToArray());
    }
}