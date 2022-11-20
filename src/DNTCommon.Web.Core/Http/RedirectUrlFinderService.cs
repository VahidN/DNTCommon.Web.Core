using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
/// Redirect Url Finder Service
/// </summary>
public class RedirectUrlFinderService : IRedirectUrlFinderService
{
    private readonly ICacheService _cacheService;
    private readonly HttpClient _client;
    private const string CachePrefix = "LocationFinder::";
    private readonly ILogger<RedirectUrlFinderService> _logger;

    static RedirectUrlFinderService()
    {
        // Default is 2 minutes: https://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.dnsrefreshtimeout(v=vs.110).aspx
        ServicePointManager.DnsRefreshTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
        // Increases the concurrent outbound connections
        ServicePointManager.DefaultConnectionLimit = 1024;
    }

    /// <summary>
    /// Redirect Url Finder Service
    /// </summary>
    public RedirectUrlFinderService(
        ICacheService cacheService,
        ILogger<RedirectUrlFinderService> logger,
        BaseHttpClient baseHttpClient)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var httpClient = baseHttpClient ?? throw new ArgumentNullException(nameof(baseHttpClient));
        _client = httpClient.HttpClient;
    }

    /// <summary>
    /// Finds the actual hidden URL after multiple redirects.
    /// </summary>
    public async Task<string?> GetRedirectUrlAsync(string siteUrl, int maxRedirects = 20)
    {
        var uri = await GetRedirectUrlAsync(new Uri(siteUrl), maxRedirects);
        return uri?.OriginalString;
    }

    /// <summary>
    /// Finds the actual hidden URL after multiple redirects.
    /// </summary>
    public async Task<Uri?> GetRedirectUrlAsync(Uri siteUri, int maxRedirects = 20)
    {
        if (siteUri == null)
        {
            throw new ArgumentNullException(nameof(siteUri));
        }

        var redirectUri = siteUri;

        try
        {
            if (_cacheService.TryGetValue($"{CachePrefix}{siteUri}", out string? outUrl) &&
                outUrl is not null)
            {
                return new Uri(outUrl);
            }

            setHeaders(siteUri);
            var hops = 1;
            do
            {
                var webResp = await _client.GetAsync(redirectUri, HttpCompletionOption.ResponseHeadersRead);
                if (webResp == null)
                {
                    return cacheReturn(siteUri, siteUri);
                }

                switch (webResp.StatusCode)
                {
                    case HttpStatusCode.Found: // 302 = HttpStatusCode.Redirect
                    case HttpStatusCode.Moved: // 301 = HttpStatusCode.MovedPermanently
                        redirectUri = webResp.Headers.Location;
                        if (redirectUri?.IsAbsoluteUri == false)
                        {
                            var leftPartAuthority = siteUri.GetComponents(UriComponents.Scheme | UriComponents.StrongAuthority, UriFormat.Unescaped);
                            redirectUri = new Uri($"{leftPartAuthority}{redirectUri}");
                        }
                        break;
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden: // fine! they have banned this server, but the link is correct!
                    case HttpStatusCode.OK:
                        return cacheReturn(siteUri, redirectUri);
                    default:
                        await webResp.EnsureSuccessStatusCodeAsync();
                        break;
                }

                hops++;
            } while (hops <= maxRedirects);

            throw new InvalidOperationException("Too many redirects detected.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.Demystify(), $"LocationFinderService error. Couldn't find redirect of {siteUri}");
        }
        catch (Exception ex) when (ex.IsNetworkError())
        {
            _logger.LogError(ex.Demystify(), $"LocationFinderService error. Couldn't find redirect of {siteUri}");
        }

        return cacheReturn(siteUri, redirectUri);
    }

    private void setHeaders(Uri siteUri)
    {
        _client.DefaultRequestHeaders.Add("User-Agent", typeof(RedirectUrlFinderService).Namespace);
        _client.DefaultRequestHeaders.Add("Keep-Alive", "true");
        _client.DefaultRequestHeaders.Referrer = siteUri;
    }

    private Uri? cacheReturn(Uri originalUrl, Uri? redirectUrl)
    {
        if (redirectUrl != null)
        {
            _cacheService.Add($"{CachePrefix}{originalUrl}", redirectUrl,
                DateTimeOffset.UtcNow.AddMinutes(15), size: 1);
        }
        return redirectUrl;
    }
}