using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Redirect Url Finder Service
/// </summary>
public class RedirectUrlFinderService : IRedirectUrlFinderService
{
    private const string CachePrefix = "LocationFinder::";
    private readonly ICacheService _cacheService;
    private readonly HttpClient _client;
    private readonly ILogger<RedirectUrlFinderService> _logger;

    /// <summary>
    ///     Redirect Url Finder Service
    /// </summary>
    public RedirectUrlFinderService(ICacheService cacheService,
        ILogger<RedirectUrlFinderService> logger,
        BaseHttpClientWithoutAutoRedirect baseHttpClient)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var httpClient = baseHttpClient ?? throw new ArgumentNullException(nameof(baseHttpClient));
        _client = httpClient.HttpClient;
    }

    /// <summary>
    ///     Finds the actual hidden URL after multiple redirects.
    /// </summary>
    public async Task<string?> GetRedirectUrlAsync(string siteUrl,
        int maxRedirects = 20,
        CancellationToken cancellationToken = default)
    {
        var uri = await GetRedirectUrlAsync(new Uri(siteUrl), maxRedirects, cancellationToken);

        return uri?.OriginalString;
    }

    /// <summary>
    ///     Finds the actual hidden URL after multiple redirects.
    /// </summary>
    public async Task<Uri?> GetRedirectUrlAsync(Uri siteUri,
        int maxRedirects = 20,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(siteUri);

        var redirectUri = siteUri;
        var hops = 1;

        try
        {
            if (_cacheService.TryGetValue($"{CachePrefix}{siteUri}", out string? outUrl) && outUrl is not null)
            {
                return new Uri(outUrl);
            }

            SetHeaders(siteUri);

            do
            {
                using var webResp = await _client.GetAsync(redirectUri, HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (webResp is null)
                {
                    return CacheReturn(siteUri, siteUri);
                }

                switch (webResp.StatusCode)
                {
                    case HttpStatusCode.Found: // 302 = HttpStatusCode.Redirect
                    case HttpStatusCode.Moved: // 301 = HttpStatusCode.MovedPermanently
                        redirectUri = webResp.Headers.Location;

                        if (redirectUri?.IsAbsoluteUri == false)
                        {
                            var leftPartAuthority =
                                siteUri.GetComponents(UriComponents.Scheme | UriComponents.StrongAuthority,
                                    UriFormat.Unescaped);

                            redirectUri = new Uri($"{leftPartAuthority}{redirectUri}");
                        }

                        break;
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden: // fine! they have banned this server, but the link is correct!
                    case HttpStatusCode.OK:
                        return CacheReturn(siteUri, redirectUri);
                    default:
                        await webResp.EnsureSuccessStatusCodeAsync(cancellationToken);

                        break;
                }

                hops++;
            }
            while (hops <= maxRedirects);

            throw new InvalidOperationException(message: "Too many redirects detected.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Demystify(),
                message: "LocationFinderService error. Couldn't find redirect of {SiteUri} after {Hops} tries.",
                siteUri, hops);
        }

        return CacheReturn(siteUri, redirectUri);
    }

    private void SetHeaders(Uri siteUri)
    {
        _client.DefaultRequestHeaders.Add(name: "User-Agent", typeof(RedirectUrlFinderService).Namespace);
        _client.DefaultRequestHeaders.Add(name: "Keep-Alive", value: "true");
        _client.DefaultRequestHeaders.Referrer = siteUri;
    }

    private Uri? CacheReturn(Uri originalUrl, Uri? redirectUrl)
    {
        if (redirectUrl is not null)
        {
            _cacheService.Add($"{CachePrefix}{originalUrl}", nameof(RedirectUrlFinderService), redirectUrl,
                DateTimeOffset.UtcNow.AddMinutes(minutes: 15));
        }

        return redirectUrl;
    }
}
