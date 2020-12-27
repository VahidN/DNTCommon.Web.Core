using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Http Request Info Extensions
    /// </summary>
    public static class SharedResourceServiceExtensions
    {
        /// <summary>
        /// Adds ISharedResourceService to IServiceCollection.
        /// How to use it: services.AddSharedResourceService of SharedResource;
        /// </summary>
        public static IServiceCollection AddSharedResourceService<T>(
                this IServiceCollection services) where T : class
        {
            services.AddScoped<IStringLocalizer>(provider => provider.GetRequiredService<IStringLocalizer<T>>());
            services.TryAddScoped<ISharedResourceService, SharedResourceService>();
            return services;
        }
    }
}