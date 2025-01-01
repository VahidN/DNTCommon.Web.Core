using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     MvcActions Discovery Service Extensions
/// </summary>
public static class MvcActionsDiscoveryServiceExtensions
{
    /// <summary>
    ///     Adds IMvcActionsDiscoveryService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddMvcActionsDiscoveryService(this IServiceCollection services)
    {
        services.AddSingleton<IMvcActionsDiscoveryService, MvcActionsDiscoveryService>();

        services.Configure<MvcOptions>(options =>
        {
            options.ModelMetadataDetailsProviders.Add(new EmptyStringEnabledDisplayMetadataProvider());
            options.AddCustomNoContentOutputFormatter();
        });

        return services;
    }

    /// <summary>
    ///     remove formatter that turns nulls into 204 - No Content responses
    ///     this formatter breaks SPA's Http response JSON parsing
    /// </summary>
    /// <param name="options"></param>
    private static void AddCustomNoContentOutputFormatter(this MvcOptions options)
    {
        options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();

        options.OutputFormatters.Insert(index: 0, new HttpNoContentOutputFormatter
        {
            TreatNullValueAsNoContent = false
        });
    }
}