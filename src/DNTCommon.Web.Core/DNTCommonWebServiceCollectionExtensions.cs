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
    ///     Sets ForwardedHeaders to ForwardedHeaders.All
    /// </summary>
    /// <param name="services"></param>
    public static void AddForwardedHeadersOptions(this IServiceCollection services)
        => services.Configure<ForwardedHeadersOptions>(options => { options.ForwardedHeaders = ForwardedHeaders.All; });

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
    public static IServiceCollection AddDNTCommonWeb(this IServiceCollection services,
        Action<ScheduledTasksStorage>? scheduledTasksOptions = null)
    {
        services.AddBackgroundQueueService();
        services.AddHttpRequestInfoService();
        services.AddRandomNumberProvider();
        services.AddEnhancedStackTraceService();
        services.AddWebMailService();
        services.AddDownloaderService();
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
        services.AddHtmlReaderService();
        services.AddAntiXssService();
        services.AddSerializationProvider();
        services.AddErrorBoundaryLoggerService();
        services.AddChromeHtmlToPngService();

        if (scheduledTasksOptions is not null)
        {
            services.AddDNTScheduler(scheduledTasksOptions);
        }

#if NET_9 || NET_8
        services.AddBlazorStaticRendererService();
#endif
        services.AddBlazorRenderingContextService();

        return services;
    }
}
