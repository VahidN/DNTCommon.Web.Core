using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     IGeminiClientService Extensions
/// </summary>
public static class GeminiClientServiceExtensions
{
    /// <summary>
    ///     Adds IGeminiClientService to IServiceCollection
    /// </summary>
    public static IServiceCollection AddGeminiClientService(this IServiceCollection services)
    {
        services.AddScoped<IGeminiClientService, GeminiClientService>();
        services.AddScoped<IGeminiLanguageAnalysisService, GeminiLanguageAnalysisService>();

        return services;
    }
}
