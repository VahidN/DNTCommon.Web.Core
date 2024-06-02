using System;
using System.Net;
using System.Threading.Tasks;

namespace DNTCommon.Web.Core;

/// <summary>
/// Url Normalization Service
/// </summary>
public class UrlNormalizationService : IUrlNormalizationService
{
    private readonly IRedirectUrlFinderService _locationFinder;

    /// <summary>
    /// Url Normalization Service
    /// </summary>
    public UrlNormalizationService(IRedirectUrlFinderService locationFinder)
    {
        _locationFinder = locationFinder ?? throw new ArgumentNullException(nameof(locationFinder));
    }

    /// <summary>
    /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
    /// </summary>
    public async Task<bool> AreTheSameUrlsAsync(string url1, string url2, bool findRedirectUrl)
    {
        url1 = await NormalizeUrlAsync(new Uri(url1), findRedirectUrl);
        url2 = await NormalizeUrlAsync(new Uri(url2), findRedirectUrl);
        return url1.Equals(url2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
    /// </summary>
    public async Task<bool> AreTheSameUrlsAsync(Uri uri1, Uri uri2, bool findRedirectUrl)
    {
        var url1 = await NormalizeUrlAsync(uri1, findRedirectUrl);
        var url2 = await NormalizeUrlAsync(uri2, findRedirectUrl);
        return url1.Equals(url2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Uses NormalizeUrl method to find the normalized URLs and then compares them.
    /// </summary>
    public bool AreTheSameUrls(Uri uri1, Uri uri2)
    {
        var url1 = NormalizeUrl(uri1);
        var url2 = NormalizeUrl(uri2);
        return url1.Equals(url2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Uses NormalizeUrl method to find the normalized URLs and then compares them.
    /// </summary>
    public bool AreTheSameUrls(string url1, string url2)
    {
        url1 = NormalizeUrl(new Uri(url1));
        url2 = NormalizeUrl(new Uri(url2));
        return url1.Equals(url2, StringComparison.OrdinalIgnoreCase);
    }

    private static readonly string[] DefaultDirectoryIndexes =
    {
        "default.asp",
        "default.aspx",
        "index.htm",
        "index.html",
        "index.php"
    };

    /// <summary>
    /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
    /// https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    public async Task<string> NormalizeUrlAsync(Uri uri, bool findRedirectUrl)
    {
        if (findRedirectUrl)
        {
            uri = await _locationFinder.GetRedirectUrlAsync(uri) ?? uri;
        }
		
        return NormalizeUrl(uri);
    }

    /// <summary>
    /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
    /// https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    public Task<string> NormalizeUrlAsync(string url, bool findRedirectUrl)
    {
        return NormalizeUrlAsync(new Uri(url), findRedirectUrl);
    }

    /// <summary>
    /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
    /// https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    public string NormalizeUrl(Uri uri)
    {
		ArgumentNullException.ThrowIfNull(uri);
		
        var url = urlToLower(uri);
        url = limitProtocols(url);
        url = removeDefaultDirectoryIndexes(url);
        url = removeTheFragment(url);
        url = removeDuplicateSlashes(url);
        url = addWww(url);
        url = removeFeedburnerPart1(url);
        url = removeFeedburnerPart2(url);
        return removeTrailingSlashAndEmptyQuery(url);
    }

    /// <summary>
    /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
    /// https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    public string NormalizeUrl(string url)
    {
        return NormalizeUrl(new Uri(url));
    }

    private static string removeFeedburnerPart1(string url)
    {
        var idx = url.IndexOf("utm_source=", StringComparison.Ordinal);
        return idx == -1 ? url : url.Substring(0, idx - 1);
    }

    private static string removeFeedburnerPart2(string url)
    {
        var idx = url.IndexOf("utm_medium=", StringComparison.Ordinal);
        return idx == -1 ? url : url.Substring(0, idx - 1);
    }

    private static string addWww(string url)
    {
        if (new Uri(url).Host.Split('.').Length == 2 && !url.Contains("://www.", StringComparison.OrdinalIgnoreCase))
        {
            return url.Replace("://", "://www.", StringComparison.OrdinalIgnoreCase);
        }
        return url;
    }

    private static string removeDuplicateSlashes(string url)
    {
        var path = new Uri(url).AbsolutePath;
        return path.Contains("//", StringComparison.OrdinalIgnoreCase) ?
            url.Replace(path, path.Replace("//", "/", StringComparison.OrdinalIgnoreCase), StringComparison.OrdinalIgnoreCase) : url;
    }

    private static string limitProtocols(string url)
    {
        return new Uri(url).Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)
            ? url.Replace("https://", "http://", StringComparison.OrdinalIgnoreCase)
            : url;
    }

    private static string removeTheFragment(string url)
    {
        var fragment = new Uri(url).Fragment;
        return string.IsNullOrWhiteSpace(fragment) ? url : url.Replace(fragment, string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static string urlToLower(Uri uri)
    {
        return WebUtility.UrlDecode(uri.AbsoluteUri.ToLowerInvariant());
    }

    private static string removeTrailingSlashAndEmptyQuery(string url)
    {
        return url
                .TrimEnd('?')
                .TrimEnd('/');
    }

    private static string removeDefaultDirectoryIndexes(string url)
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
}