using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     MiscUtils Extensions
/// </summary>
public static class MiscUtilsServiceExtensions
{
    /// <summary>
    ///     Adds MiscUtils to IServiceCollection
    /// </summary>
    public static IServiceCollection AddMiscUtilsService(this IServiceCollection services)
    {
        services.AddSingleton<ILockerService, LockerService>();

        return services;
    }
}