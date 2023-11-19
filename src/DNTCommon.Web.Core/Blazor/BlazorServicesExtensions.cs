using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DNTCommon.Web.Core;

/// <summary>
///     Adds Blazor related services to IServiceCollection.
/// </summary>
public static class BlazorServicesExtensions
{
#if NET_8
    /// <summary>
    ///     Adds IQueueBackgroundWorkItem to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddBlazorStaticRendererService(this IServiceCollection services)
    {
        services.TryAddScoped<HtmlRenderer>();
        services.TryAddScoped<IBlazorStaticRendererService, BlazorStaticRendererService>();
        return services;
    }
#endif
}