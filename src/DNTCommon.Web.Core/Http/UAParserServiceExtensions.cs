using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Html Helper Service Extensions
/// </summary>
public static class UAParserServiceExtensions
{
    /// <summary>
    ///     Adds IUAParserService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddUAParserService(this IServiceCollection services)
    {
        services.AddSingleton<IUAParserService, UAParserService>();

        return services;
    }
}