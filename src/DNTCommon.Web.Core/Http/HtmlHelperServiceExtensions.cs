using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
/// Html Helper Service Extensions
/// </summary>
public static class HtmlHelperServiceExtensions
{
    /// <summary>
    /// Adds IHtmlHelperService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddHtmlHelperService(this IServiceCollection services)
    {
        services.AddScoped<IHtmlHelperService, HtmlHelperService>();
        return services;
    }
}