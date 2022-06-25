using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
/// MvcActions Discovery Service Extensions
/// </summary>
public static class MvcActionsDiscoveryServiceExtensions
{
    /// <summary>
    /// Adds IMvcActionsDiscoveryService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddMvcActionsDiscoveryService(this IServiceCollection services)
    {
        services.AddSingleton<IMvcActionsDiscoveryService, MvcActionsDiscoveryService>();
        return services;
    }
}