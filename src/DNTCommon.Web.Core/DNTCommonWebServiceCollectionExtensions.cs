using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DNTCommon.Web.Core;

/// <summary>
///     ServiceCollection Extensions
/// </summary>
public static class DntCommonWebServiceCollectionExtensions
{
    /// <summary>
    ///     Sets ServicesStartConcurrently and ServicesStopConcurrently to true
    /// </summary>
    public static void RunHostedServicesConcurrently(this IServiceCollection services)
        => services.Configure<HostOptions>(options =>
        {
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = true;
        });

    /// <summary>
    ///     Sets ForwardedHeaders to ForwardedHeaders.All
    /// </summary>
    /// <param name="services"></param>
    /// <param name="trustLocalNginx">
    ///     Configure ForwardedHeaders to trust the local nginx (localhost). The local nginx proxy
    ///     running on the same machine.
    /// </param>
    /// <param name="trustedNginxHosts">Addresses of known proxies to accept forwarded headers from.</param>
    public static void AddForwardedHeadersOptions(this IServiceCollection services,
        bool trustLocalNginx = true,
        params ICollection<IPAddress>? trustedNginxHosts)
        => services.Configure<ForwardedHeadersOptions>(options =>
        {
            // We want X-Forwarded-For (client IP chain) and X-Forwarded-Proto (scheme)
            options.ForwardedHeaders = ForwardedHeaders.All;

            if (trustLocalNginx)
            {
                // Trust the local nginx proxy running on the same machine.
                options.KnownProxies.Add(IPAddress.Loopback); // 127.0.0.1
                options.KnownProxies.Add(IPAddress.IPv6Loopback); // ::1
            }

            if (trustedNginxHosts?.Count > 0)
            {
                foreach (var trustedNginxHost in trustedNginxHosts)
                {
                    options.KnownProxies.Add(trustedNginxHost);
                }
            }
        });

    /// <summary>
    ///     Performs check verifying that scoped services never gets resolved from root provider.
    ///     Performs check verifying that all services can be created during BuildServiceProvider call
    /// </summary>
    public static void AlwaysValidateScopes(this IHostBuilder host)
        => host.UseDefaultServiceProvider(options =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });

    /// <summary>
    ///     Adds all the default providers of DNTCommon.Web.Core at once.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="scheduledTasksOptions">Scheduled Tasks Storage options</param>
    /// <param name="autoInjectAllServices">
    ///     Adds services which implement IScopedService, ISingletonService, IMiddleware, ITransientService and
    ///     BackgroundService to the service collection from the specified assemblies.
    /// </param>
    /// <param name="injectServicesFromAssemblies">The preferred services assemblies</param>
    /// <returns></returns>
    public static IServiceCollection AddDNTCommonWeb(this IServiceCollection services,
        Action<ScheduledTasksStorage>? scheduledTasksOptions = null,
        bool autoInjectAllServices = false,
        params ICollection<Assembly>? injectServicesFromAssemblies)
    {
        services.AddBackgroundQueueService();
        services.AddHttpRequestInfoService();
        services.AddRandomNumberProvider();
        services.AddEnhancedStackTraceService();
        services.AddWebMailService();
        services.AddDownloaderService();
        services.AddRssReaderService();
        services.AddGeminiClientService();
        services.AddMiscUtilsService();
        services.AddBaseHttpClient();
        services.AddRedirectUrlFinderService();
        services.AddMemoryCacheService();
        services.AddMvcActionsDiscoveryService();
        services.AddDesProviderService();
        services.AddProtectionProviderService();
        services.AddPasswordHasherService();
        services.AddHtmlHelperService();
        services.AddUAParserService();
        services.AddFileNameSanitizerService();
        services.AddUrlNormalizationService();
        services.AddRazorViewRenderer();
        services.AddCommonHttpClientFactory();
        services.AddUploadFileService();
        services.AddAntiDosFirewall();
        services.AddTimeProviderService();
        services.AddAsyncLocalCancellationContext();
        services.AddHtmlReaderService();
        services.AddAntiXssService();
        services.AddSerializationProvider();
        services.AddErrorBoundaryLoggerService();
        services.AddChromeHtmlToPngService();
        services.AddCollectionsServices();

        if (scheduledTasksOptions is not null)
        {
            services.AddDNTScheduler(scheduledTasksOptions);
        }

        services.AddBlazorStaticRendererService();
        services.AddBlazorRenderingContextService();

        if (autoInjectAllServices)
        {
            services.AutoInjectAllServices(injectServicesFromAssemblies);
        }

        return services;
    }
}
