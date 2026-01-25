using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Timeout;

namespace DNTCommon.Web.Core;

/// <summary>
///     BaseHttpClient Extensions
/// </summary>
public static class BaseHttpClientExtensions
{
    /// <summary>
    ///     Adds BaseHttpClient to IServiceCollection
    /// </summary>
    public static IServiceCollection AddBaseHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<BaseHttpClient>(AddDefaultSettings)
            .ConfigurePrimaryHttpMessageHandler(() => CreateScrapingHandler(allowAutoRedirect: true))
            .AddPolicyHandler(GetTimeoutPolicy())
            .AddPolicyHandler(GetRetryPolicy())
#if !NET_6
            .AddPolicyHandler(GetRateLimitPolicy())
#endif
            ;

        services.AddHttpClient<BaseHttpClientWithoutAutoRedirect>(AddDefaultSettings)
            .ConfigurePrimaryHttpMessageHandler(() => CreateScrapingHandler(allowAutoRedirect: false))
            .AddPolicyHandler(GetTimeoutPolicy())
            .AddPolicyHandler(GetRetryPolicy())
#if !NET_6
            .AddPolicyHandler(GetRateLimitPolicy())
#endif
            ;

        services.RemoveAll<IHttpMessageHandlerBuilderFilter>(); // Remove logging of the HttpClient

        return services;
    }

    private static void AddDefaultSettings(HttpClient client)
    {
        client.DefaultRequestHeaders.Clear();

        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgentPool.Random());

        client.DefaultRequestHeaders.Accept.ParseAdd(
            input: "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(input: "en-US,en;q=0.9");
        client.DefaultRequestHeaders.AcceptEncoding.ParseAdd(input: "gzip, deflate");

        client.DefaultRequestHeaders.ConnectionClose = false;
    }

    private static AsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        => Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(value: 30), TimeoutStrategy.Optimistic);

    private static AsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        => Policy<HttpResponseMessage>.Handle<HttpRequestException>()
            .Or<TaskCanceledException>() // network stall
            .OrResult(r => (int)r.StatusCode >= 500 || r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(retryCount: 3,
                retry => TimeSpan.FromSeconds(Math.Pow(x: 2, retry)) +
                         TimeSpan.FromMilliseconds(RandomNumberGenerator.GetInt32(fromInclusive: 100,
                             toExclusive: 500)));

#if !NET_6
    private static AsyncPolicy<HttpResponseMessage> GetRateLimitPolicy()
        => Policy.RateLimitAsync<HttpResponseMessage>(numberOfExecutions: 10, TimeSpan.FromSeconds(value: 1));
#endif

    private static SocketsHttpHandler CreateScrapingHandler(bool allowAutoRedirect)
        => new()
        {
            AutomaticDecompression = DecompressionMethods.All,
            PooledConnectionLifetime = TimeSpan.FromMinutes(value: 10),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(value: 2),
            MaxConnectionsPerServer = 50,
            ConnectTimeout = TimeSpan.FromSeconds(value: 15),
            KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
            KeepAlivePingDelay = TimeSpan.FromSeconds(value: 30),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(value: 10),
            UseCookies = true,
            CookieContainer = new CookieContainer(),
            AllowAutoRedirect = allowAutoRedirect,
            MaxAutomaticRedirections = 5 // Limit to prevent infinite loops
        };
}
