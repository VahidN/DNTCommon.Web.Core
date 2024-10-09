using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Url Normalization Service Extensions
/// </summary>
public static class UrlNormalizationServiceExtensions
{
    private static readonly string[] DefaultDirectoryIndexes =
    [
        "default.asp", "default.aspx", "index.htm", "index.html", "index.php"
    ];

    /// <summary>
    ///     Adds IUrlNormalizationService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddUrlNormalizationService(this IServiceCollection services)
    {
        services.AddSingleton<IUrlNormalizationService, UrlNormalizationService>();

        return services;
    }

    /// <summary>
    ///     URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of
    ///     the normalization process is to transform a URL into a normalized URL so it is possible to determine if two
    ///     syntactically different URLs may be equivalent.
    ///     https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    public static string NormalizeUrl(this Uri uri,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All)
    {
        ArgumentNullException.ThrowIfNull(uri);

        var url = WebUtility.UrlDecode(uri.AbsoluteUri);

        if (normalizeUrlRules.HasFlag(NormalizeUrlRules.UrlToLower))
        {
            url = UrlToLower(url);
        }

        if (normalizeUrlRules.HasFlag(NormalizeUrlRules.LimitProtocols))
        {
            url = LimitProtocols(url, defaultProtocol);
        }

        if (normalizeUrlRules.HasFlag(NormalizeUrlRules.RemoveDefaultDirectoryIndexes))
        {
            url = RemoveDefaultDirectoryIndexes(url);
        }

        if (normalizeUrlRules.HasFlag(NormalizeUrlRules.RemoveTheFragment))
        {
            url = RemoveTheFragment(url);
        }

        if (normalizeUrlRules.HasFlag(NormalizeUrlRules.RemoveDuplicateSlashes))
        {
            url = RemoveDuplicateSlashes(url);
        }

        if (normalizeUrlRules.HasFlag(NormalizeUrlRules.AddWww))
        {
            url = AddWww(url);
        }

        if (normalizeUrlRules.HasFlag(NormalizeUrlRules.RemoveFeedburnerPart1))
        {
            url = RemoveFeedburnerPart1(url);
        }

        if (normalizeUrlRules.HasFlag(NormalizeUrlRules.RemoveFeedburnerPart2))
        {
            url = RemoveFeedburnerPart2(url);
        }

        if (normalizeUrlRules.HasFlag(NormalizeUrlRules.RemoveTrailingSlashAndEmptyQuery))
        {
            url = RemoveTrailingSlashAndEmptyQuery(url);
        }

        return url;
    }

    /// <summary>
    ///     URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of
    ///     the normalization process is to transform a URL into a normalized URL so it is possible to determine if two
    ///     syntactically different URLs may be equivalent.
    ///     https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    public static string NormalizeUrl(this string url,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All)
        => NormalizeUrl(new Uri(url), defaultProtocol, normalizeUrlRules);

    private static string RemoveFeedburnerPart1(string url)
    {
        var idx = url.IndexOf(value: "utm_source=", StringComparison.Ordinal);

        return idx == -1 ? url : url.Substring(startIndex: 0, idx - 1);
    }

    private static string RemoveFeedburnerPart2(string url)
    {
        var idx = url.IndexOf(value: "utm_medium=", StringComparison.Ordinal);

        return idx == -1 ? url : url.Substring(startIndex: 0, idx - 1);
    }

    private static string AddWww(string url)
    {
        if (new Uri(url).Host.Split(separator: '.').Length == 2 &&
            !url.Contains(value: "://www.", StringComparison.OrdinalIgnoreCase))
        {
            return url.Replace(oldValue: "://", newValue: "://www.", StringComparison.OrdinalIgnoreCase);
        }

        return url;
    }

    private static string RemoveDuplicateSlashes(string url)
    {
        var path = new Uri(url).AbsolutePath;

        return path.Contains(value: "//", StringComparison.OrdinalIgnoreCase)
            ? url.Replace(path, path.Replace(oldValue: "//", newValue: "/", StringComparison.OrdinalIgnoreCase),
                StringComparison.OrdinalIgnoreCase)
            : url;
    }

    private static string LimitProtocols(string url, string defaultProtocol)
    {
        var scheme = new Uri(url).Scheme;

        return scheme.Equals(defaultProtocol, StringComparison.OrdinalIgnoreCase)
            ? url
            : url.Replace($"{scheme}://", $"{defaultProtocol}://", StringComparison.OrdinalIgnoreCase);
    }

    private static string RemoveTheFragment(string url)
    {
        var fragment = new Uri(url).Fragment;

        return string.IsNullOrWhiteSpace(fragment)
            ? url
            : url.Replace(fragment, string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static string UrlToLower(string url) => url.ToLowerInvariant();

    private static string RemoveTrailingSlashAndEmptyQuery(string url)
        => url.TrimEnd(trimChar: '?').TrimEnd(trimChar: '/');

    private static string RemoveDefaultDirectoryIndexes(string url)
    {
        foreach (var index in DefaultDirectoryIndexes)
        {
            if (url.EndsWith(index, StringComparison.OrdinalIgnoreCase))
            {
                url = url.TrimEnd(index.ToCharArray());

                break;
            }
        }

        return url;
    }

    /// <summary>
    ///     Uses NormalizeUrl method to find the normalized URLs and then compares them.
    /// </summary>
    public static bool AreTheSameUrls(this Uri uri1,
        Uri uri2,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All)
    {
        var url1 = NormalizeUrl(uri1, defaultProtocol, normalizeUrlRules);
        var url2 = NormalizeUrl(uri2, defaultProtocol, normalizeUrlRules);

        return url1.Equals(url2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Uses NormalizeUrl method to find the normalized URLs and then compares them.
    /// </summary>
    public static bool AreTheSameUrls(this string url1,
        string url2,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All)
    {
        url1 = NormalizeUrl(new Uri(url1), defaultProtocol, normalizeUrlRules);
        url2 = NormalizeUrl(new Uri(url2), defaultProtocol, normalizeUrlRules);

        return url1.Equals(url2, StringComparison.OrdinalIgnoreCase);
    }
}