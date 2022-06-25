using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Polly;

namespace DNTCommon.Web.Core;

/// <summary>
/// BaseHttpClient Extensions
/// </summary>
public static class BaseHttpClientExtensions
{
    /// <summary>
    /// Gets the cookie container used to store server cookies by the handler.
    /// </summary>
    /// <returns></returns>
    public static readonly CookieContainer CookieContainer = new CookieContainer();

    private static readonly HttpClientHandler _httpClientHandler = new HttpClientHandler
    {
        AllowAutoRedirect = false,
        CookieContainer = CookieContainer,
        UseCookies = true,
        AutomaticDecompression = DecompressionMethods.All
    };

    /// <summary>
    /// Adds BaseHttpClient to IServiceCollection
    /// </summary>
    public static IServiceCollection AddBaseHttpClient(this IServiceCollection services)
    {
        var httpClientBuilder = services.AddHttpClient<BaseHttpClient>(
            client =>
            {
                client.Timeout = TimeSpan.FromMinutes(3);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:75.0) Gecko/20100101 Firefox/75.0");
                client.DefaultRequestHeaders.Add("Keep-Alive", "true");
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("TE", "Trailers");
            }).AddTransientHttpErrorPolicy(policy =>
                // transient errors: network failures and HTTP 5xx and HTTP 408 errors
                policy.WaitAndRetryAsync(new[]
                                        {
                                                        TimeSpan.FromSeconds(3),
                                                        TimeSpan.FromSeconds(5),
                                                        TimeSpan.FromSeconds(15)
                                        }));
        httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => _httpClientHandler);
        services.RemoveAll<IHttpMessageHandlerBuilderFilter>(); // Remove logging of the HttpClient
        return services;
    }
}