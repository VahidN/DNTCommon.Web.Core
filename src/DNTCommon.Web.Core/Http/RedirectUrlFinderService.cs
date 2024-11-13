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

    static RedirectUrlFinderService()
    {
        // Default is 2 minutes: https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.dnsrefreshtimeout(v=vs.110).aspx
        ServicePointManager.DnsRefreshTimeout = (int)TimeSpan.FromMinutes(value: 1).TotalMilliseconds;

        // Increases the concurrent outbound connections
        ServicePointManager.DefaultConnectionLimit = 1024;
    }

    /// <summary>
    ///     Redirect Url Finder Service
    /// </summary>
    public RedirectUrlFinderService(ICacheService cacheService,
        ILogger<RedirectUrlFinderService> logger,
        BaseHttpClient baseHttpClient)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var httpClient = baseHttpClient ?? throw new ArgumentNullException(nameof(baseHttpClient));
        _client = httpClient.HttpClient;
    }

    /// <summary>
    ///     Finds the actual hidden URL after multiple redirects.
    /// </summary>
    public async Task<string?> GetRedirectUrlAsync(string siteUrl, int maxRedirects = 20)
    {
        var uri = await GetRedirectUrlAsync(new Uri(siteUrl), maxRedirects);

        return uri?.OriginalString;
    }

    /// <summary>
    ///     Finds the actual hidden URL after multiple redirects.
    /// </summary>
    public async Task<Uri?> GetRedirectUrlAsync(Uri siteUri, int maxRedirects = 20)
    {
        if (siteUri == null)
        {
            throw new ArgumentNullException(nameof(siteUri));
        }

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
                var webResp = await _client.GetAsync(redirectUri, HttpCompletionOption.ResponseHeadersRead);

                if (webResp == null)
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
                        await webResp.EnsureSuccessStatusCodeAsync();

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
        if (redirectUrl != null)
        {
            _cacheService.Add($"{CachePrefix}{originalUrl}", redirectUrl,
                DateTimeOffset.UtcNow.AddMinutes(minutes: 15));
        }

        return redirectUrl;
    }
}