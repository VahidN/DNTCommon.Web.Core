using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Polly;

namespace DNTCommon.Web.Core;

/// <summary>
///     BaseHttpClient Extensions
/// </summary>
public static class BaseHttpClientExtensions
{
    /// <summary>
    ///     Gets the cookie container used to store server cookies by the handler.
    /// </summary>
    /// <returns></returns>
    public static readonly CookieContainer CookieContainer = new();

    /// <summary>
    ///     Adds BaseHttpClient to IServiceCollection
    /// </summary>
    public static IServiceCollection AddBaseHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<BaseHttpClient>(AddDefaultSettings)
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync([
                // transient errors: network failures and HTTP 5xx and HTTP 408 errors
                TimeSpan.FromSeconds(value: 3)
            ]))
            .ConfigurePrimaryHttpMessageHandler(() => GetDefaultSocketsHttpHandler(allowAutoRedirect: true));

        services.AddHttpClient<BaseHttpClientWithoutAutoRedirect>(AddDefaultSettings)
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync([
                // transient errors: network failures and HTTP 5xx HTTP 408 errors
                TimeSpan.FromSeconds(value: 3)
            ]))
            .ConfigurePrimaryHttpMessageHandler(() => GetDefaultSocketsHttpHandler(allowAutoRedirect: false));

        services.RemoveAll<IHttpMessageHandlerBuilderFilter>(); // Remove logging of the HttpClient

        return services;
    }

    /// <summary>
    ///     We should not supply the factory with a single instance of HttpClientHandler to use in all clients.
    ///     Once the default HandlerLifetime has elapsed (1 minutes) it will be marked for disposal,
    ///     with the actual disposal occurring after all existing HttpClients referencing it are disposed.
    ///     All clients created after the handler is marked continue to be supplied the soon-to-be disposed handler,
    ///     leaving them in an invalid state once the disposal is actioned.
    ///     To fix this, the factory should be configured to create a new handler for each client.
    /// </summary>
    /// <returns></returns>
    private static SocketsHttpHandler GetDefaultSocketsHttpHandler(bool allowAutoRedirect)
        => new()
        {
            AllowAutoRedirect = allowAutoRedirect,
            CookieContainer = CookieContainer,
            UseCookies = true,
            AutomaticDecompression = DecompressionMethods.All,
            PooledConnectionLifetime = TimeSpan.FromSeconds(value: 10),
            PooledConnectionIdleTimeout = TimeSpan.FromSeconds(value: 10),
            MaxConnectionsPerServer = 200,
            ConnectTimeout = TimeSpan.FromSeconds(value: 10),
            KeepAlivePingDelay = TimeSpan.FromSeconds(value: 10)
        };

    private static void AddDefaultSettings(HttpClient client)
    {
        client.Timeout = TimeSpan.FromSeconds(value: 10);

        client.DefaultRequestHeaders.Add(name: "User-Agent",
            value:
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/143.0.0.0 Safari/537.36");

        client.DefaultRequestHeaders.Add(name: "Keep-Alive", value: "true");

        client.DefaultRequestHeaders.Add(name: "Accept",
            value: "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

        client.DefaultRequestHeaders.Add(name: "Accept-Language", value: "en-US,en;q=0.5");
        client.DefaultRequestHeaders.Add(name: "Accept-Encoding", value: "gzip, deflate, br");
        client.DefaultRequestHeaders.Add(name: "TE", value: "Trailers");
    }
}
