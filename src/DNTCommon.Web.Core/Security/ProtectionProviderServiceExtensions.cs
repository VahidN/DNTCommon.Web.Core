using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     More info: http://www.dntips.ir/post/2519
/// </summary>
public static class ProtectionProviderServiceExtensions
{
    /// <summary>
    ///     Adds IProtectionProviderService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddProtectionProviderService(this IServiceCollection services)
    {
        services.AddSingleton<IProtectionProviderService, ProtectionProviderService>();

        return services;
    }
}