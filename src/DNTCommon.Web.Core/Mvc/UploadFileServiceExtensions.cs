using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Upload File Service Extensions
    /// </summary>
    public static class UploadFileServiceExtensions
    {
        /// <summary>
        /// Adds IUploadFileService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddUploadFileService(this IServiceCollection services)
        {
            services.AddSingleton<IUploadFileService, UploadFileService>();
            return services;
        }
    }
}