using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     ServiceCollection Extensions
/// </summary>
public static class DntCommonWebServiceCollectionExtensions
{
    /// <summary>
    ///     Adds all of the default providers of DNTCommon.Web.Core at once.
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

        if (scheduledTasksOptions != null)
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