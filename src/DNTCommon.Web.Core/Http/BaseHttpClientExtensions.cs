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
            .AddPolicyHandler(GetRateLimitPolicy());

        services.AddHttpClient<BaseHttpClientWithoutAutoRedirect>(AddDefaultSettings)
            .ConfigurePrimaryHttpMessageHandler(() => CreateScrapingHandler(allowAutoRedirect: false))
            .AddPolicyHandler(GetTimeoutPolicy())
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetRateLimitPolicy());

        services.RemoveAll<IHttpMessageHandlerBuilderFilter>(); // Remove logging of the HttpClient

        return services;
    }

    private static void AddDefaultSettings(HttpClient client)
    {
        client.DefaultRequestHeaders.UserAgent.ParseAdd(input: "DNTCommon/1.0");
        client.DefaultRequestHeaders.Accept.ParseAdd(input: "text/html,application/xhtml+xml");
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(input: "en-US,en;q=0.9");
        client.DefaultRequestHeaders.AcceptEncoding.ParseAdd(input: "gzip, deflate");
    }

    private static AsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        => Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(seconds: 30), TimeoutStrategy.Optimistic);

    private static AsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        => Policy<HttpResponseMessage>.Handle<HttpRequestException>()
            .Or<TaskCanceledException>() // network stall
            .OrResult(r => (int)r.StatusCode >= 500 || r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(retryCount: 3,
                retry => TimeSpan.FromSeconds(Math.Pow(x: 2, retry)) +
                         TimeSpan.FromMilliseconds(RandomNumberGenerator.GetInt32(fromInclusive: 100,
                             toExclusive: 500)));

    private static AsyncPolicy<HttpResponseMessage> GetRateLimitPolicy()
        => Policy.RateLimitAsync<HttpResponseMessage>(numberOfExecutions: 10, TimeSpan.FromSeconds(seconds: 1));

    private static SocketsHttpHandler CreateScrapingHandler(bool allowAutoRedirect)
        => new()
        {
            AutomaticDecompression = DecompressionMethods.All,
            PooledConnectionLifetime = TimeSpan.FromMinutes(minutes: 10),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(minutes: 2),
            MaxConnectionsPerServer = 50,
            ConnectTimeout = TimeSpan.FromSeconds(seconds: 15),
            KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
            KeepAlivePingDelay = TimeSpan.FromSeconds(seconds: 30),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(seconds: 10),
            UseCookies = false,
            AllowAutoRedirect = allowAutoRedirect
        };
}
