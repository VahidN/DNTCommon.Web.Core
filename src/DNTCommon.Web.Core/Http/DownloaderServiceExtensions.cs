using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// DownloaderService Extensions
    /// </summary>
    public static class DownloaderServiceExtensions
    {
        /// <summary>
        /// Adds IDownloaderService to IServiceCollection
        /// </summary>
        public static IServiceCollection AddDownloaderService(this IServiceCollection services)
        {
            services.AddTransient<IDownloaderService, DownloaderService>();
            return services;
        }
    }
}