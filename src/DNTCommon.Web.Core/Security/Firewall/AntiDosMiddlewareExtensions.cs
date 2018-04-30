using Microsoft.AspNetCore.Builder;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// AntiDos Middleware Extensions
    /// </summary>
    public static class AntiDosMiddlewareExtensions
    {
        /// <summary>
        /// Adds AntiDosMiddleware to the pipeline.
        /// </summary>
        public static IApplicationBuilder UseAntiDos(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiDosMiddleware>();
        }
    }
}