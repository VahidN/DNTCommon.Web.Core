using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Adds Custom Services to IServiceCollection.
/// </summary>
public static class CollectionsServicesExtensions
{
    /// <summary>
    ///     Adds IBagCacheService Services to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddCollectionsServices(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IBagCacheService<>), typeof(BagCacheService<>));
        services.AddSingleton(typeof(ILockedDictionaryCacheService<>), typeof(LockedDictionaryCacheService<>));

        return services;
    }
}
