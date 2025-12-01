namespace DNTCommon.Web.Core;

/// <summary>
///     Url Normalization Service
/// </summary>
/// <remarks>
///     Url Normalization Service
/// </remarks>
public class UrlNormalizationService(IRedirectUrlFinderService locationFinder) : IUrlNormalizationService
{
    private readonly IRedirectUrlFinderService _locationFinder =
        locationFinder ?? throw new ArgumentNullException(nameof(locationFinder));

    /// <summary>
    ///     Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
    /// </summary>
    public async Task<bool> AreTheSameUrlsAsync(string url1,
        string url2,
        bool findRedirectUrl,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All,
        CancellationToken cancellationToken = default)
    {
        url1 = await NormalizeUrlAsync(new Uri(url1), findRedirectUrl, defaultProtocol, normalizeUrlRules,
            cancellationToken);

        url2 = await NormalizeUrlAsync(new Uri(url2), findRedirectUrl, defaultProtocol, normalizeUrlRules,
            cancellationToken);

        return url1.Equals(url2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
    /// </summary>
    public async Task<bool> AreTheSameUrlsAsync(Uri uri1,
        Uri uri2,
        bool findRedirectUrl,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All,
        CancellationToken cancellationToken = default)
    {
        var url1 = await NormalizeUrlAsync(uri1, findRedirectUrl, defaultProtocol, normalizeUrlRules,
            cancellationToken);

        var url2 = await NormalizeUrlAsync(uri2, findRedirectUrl, defaultProtocol, normalizeUrlRules,
            cancellationToken);

        return url1.Equals(url2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of
    ///     the normalization process is to transform a URL into a normalized URL so it is possible to determine if two
    ///     syntactically different URLs may be equivalent.
    ///     https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    public async Task<string> NormalizeUrlAsync(Uri uri,
        bool findRedirectUrl,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All,
        CancellationToken cancellationToken = default)
    {
        if (findRedirectUrl)
        {
            uri = await _locationFinder.GetRedirectUrlAsync(uri, cancellationToken: cancellationToken) ?? uri;
        }

        return uri.NormalizeUrl(defaultProtocol, normalizeUrlRules);
    }

    /// <summary>
    ///     URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of
    ///     the normalization process is to transform a URL into a normalized URL so it is possible to determine if two
    ///     syntactically different URLs may be equivalent.
    ///     https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    public Task<string> NormalizeUrlAsync(string url,
        bool findRedirectUrl,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All,
        CancellationToken cancellationToken = default)
        => NormalizeUrlAsync(new Uri(url), findRedirectUrl, defaultProtocol, normalizeUrlRules, cancellationToken);

    /// <summary>
    ///     Uses NormalizeUrl method to find the normalized URLs and then compares them.
    /// </summary>
    public bool AreTheSameUrls(Uri uri1,
        Uri uri2,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All)
        => uri1.AreTheSameUrls(uri2, defaultProtocol, normalizeUrlRules);

    /// <summary>
    ///     Uses NormalizeUrl method to find the normalized URLs and then compares them.
    /// </summary>
    public bool AreTheSameUrls(string url1,
        string url2,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All)
        => url1.AreTheSameUrls(url2, defaultProtocol, normalizeUrlRules);

    /// <summary>
    ///     URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of
    ///     the normalization process is to transform a URL into a normalized URL so it is possible to determine if two
    ///     syntactically different URLs may be equivalent.
    ///     https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    public string NormalizeUrl(Uri uri,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All)
        => uri.NormalizeUrl(defaultProtocol, normalizeUrlRules);

    /// <summary>
    ///     URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of
    ///     the normalization process is to transform a URL into a normalized URL so it is possible to determine if two
    ///     syntactically different URLs may be equivalent.
    ///     https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    public string NormalizeUrl(string url,
        string defaultProtocol = "http",
        NormalizeUrlRules normalizeUrlRules = NormalizeUrlRules.All)
        => url.NormalizeUrl(defaultProtocol, normalizeUrlRules);
}
