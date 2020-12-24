using System;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// ServiceCollection Extensions
    /// </summary>
    public static class DNTCommonWebServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all of the default providers of DNTCommon.Web.Core at once.
        /// </summary>
        public static IServiceCollection AddDNTCommonWeb(
            this IServiceCollection services,
            Action<ScheduledTasksStorage>? scheduledTasksOptions = null)
        {
            services.AddBackgroundQueueService();
            services.AddHttpRequestInfoService();
            services.AddRandomNumberProvider();
            services.AddWebMailService();
            services.AddDownloaderService();
            services.AddBaseHttpClient();
            services.AddRedirectUrlFinderService();
            services.AddMemoryCacheService();
            services.AddMvcActionsDiscoveryService();
            services.AddDesProviderService();
            services.AddProtectionProviderService();
            services.AddPasswordHasherService();
            services.AddHtmlHelperService();
            services.AddFileNameSanitizerService();
            services.AddUrlNormalizationService();
            services.AddRazorViewRenderer();
            services.AddCommonHttpClientFactory();
            services.AddUploadFileService();
            services.AddAntiDosFirewall();
            services.AddHtmlReaderService();
            services.AddAntiXssService();
            services.AddSerializationProvider();

            if (scheduledTasksOptions != null)
            {
                services.AddDNTScheduler(scheduledTasksOptions);
            }

            return services;
        }
    }
}