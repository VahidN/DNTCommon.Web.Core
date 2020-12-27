using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// HtmlReaderService Extensions
    /// </summary>
    public static class HtmlReaderServiceExtensions
    {
        /// <summary>
        /// Adds IHtmlReaderService to IServiceCollection
        /// </summary>
        public static IServiceCollection AddHtmlReaderService(this IServiceCollection services)
        {
            services.AddTransient<IHtmlReaderService, HtmlReaderService>();
            return services;
        }
    }
}