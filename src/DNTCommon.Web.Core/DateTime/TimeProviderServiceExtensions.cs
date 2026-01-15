using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     TimeProvider Service Extensions
/// </summary>
public static class TimeProviderServiceExtensions
{
    /// <summary>
    ///     Adds ITimeProvider to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddTimeProviderService(this IServiceCollection services)
    {
        services.AddSingleton<ISystemTimeProvider, SystemTimeProvider>();

        return services;
    }
}
