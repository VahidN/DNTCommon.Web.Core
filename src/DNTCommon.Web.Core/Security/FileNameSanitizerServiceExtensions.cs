using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// SafeFile Download Service Extensions
    /// </summary>
    public static class FileNameSanitizerServiceExtensions
    {
        /// <summary>
        /// Adds IFileNameSanitizerService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddFileNameSanitizerService(this IServiceCollection services)
        {
            services.AddTransient<IFileNameSanitizerService, FileNameSanitizerService>();
            return services;
        }
    }
}