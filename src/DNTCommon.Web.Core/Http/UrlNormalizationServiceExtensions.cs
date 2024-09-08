using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Url Normalization Service Extensions
/// </summary>
public static class UrlNormalizationServiceExtensions
{
    /// <summary>
    ///     Adds IUrlNormalizationService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddUrlNormalizationService(this IServiceCollection services)
    {
        services.AddSingleton<IUrlNormalizationService, UrlNormalizationService>();

        return services;
    }
}