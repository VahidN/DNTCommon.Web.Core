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
        var httpClientBuilder = services.AddHttpClient<BaseHttpClient>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(value: 20);

                client.DefaultRequestHeaders.Add(name: "User-Agent",
                    value:
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36");

                client.DefaultRequestHeaders.Add(name: "Keep-Alive", value: "true");

                client.DefaultRequestHeaders.Add(name: "Accept",
                    value: "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

                client.DefaultRequestHeaders.Add(name: "Accept-Language", value: "en-US,en;q=0.5");
                client.DefaultRequestHeaders.Add(name: "Accept-Encoding", value: "gzip, deflate, br");
                client.DefaultRequestHeaders.Add(name: "TE", value: "Trailers");
            })
            .AddTransientHttpErrorPolicy(policy =>

                // transient errors: network failures and HTTP 5xx and HTTP 408 errors
                policy.WaitAndRetryAsync([
                    TimeSpan.FromSeconds(value: 3), TimeSpan.FromSeconds(value: 5), TimeSpan.FromSeconds(value: 15)
                ]));

        /*
         We should not supply the factory with a single instance of HttpClientHandler to use in all clients.
        Once the default HandlerLifetime has elapsed (2 minutes) it will be marked for disposal,
        with the actual disposal occurring after all existing HttpClients referencing it are disposed.
        All clients created after the handler is marked continue to be supplied the soon-to-be disposed handler,
        leaving them in an invalid state once the disposal is actioned.
        To fix this, the factory should be configured to create a new handler for each client.
         */
        httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            CookieContainer = CookieContainer,
            UseCookies = true,
            AutomaticDecompression = DecompressionMethods.All,
            PooledConnectionLifetime = TimeSpan.FromMinutes(minutes: 2),
            PooledConnectionIdleTimeout = TimeSpan.FromSeconds(seconds: 30),
            MaxConnectionsPerServer = 100
        });

        services.RemoveAll<IHttpMessageHandlerBuilderFilter>(); // Remove logging of the HttpClient

        return services;
    }
}
