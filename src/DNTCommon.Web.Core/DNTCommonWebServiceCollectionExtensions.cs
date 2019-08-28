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
        public static IServiceCollection AddDNTCommonWeb(this IServiceCollection services)
        {
            services.AddHttpRequestInfoService();
            services.AddWebMailService();
            services.AddDownloaderService();
            services.AddRedirectUrlFinderService();
            services.AddMemoryCacheService();
            services.AddMvcActionsDiscoveryService();
            services.AddProtectionProviderService();
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
            return services;
        }
    }
}