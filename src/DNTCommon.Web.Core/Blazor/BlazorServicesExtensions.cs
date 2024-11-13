using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DNTCommon.Web.Core;

/// <summary>
///     Adds Blazor related services to IServiceCollection.
/// </summary>
public static class BlazorServicesExtensions
{
#if NET_9 || NET_8
    /// <summary>
    ///     Adds services to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddBlazorStaticRendererService(this IServiceCollection services)
    {
        services.TryAddScoped<Microsoft.AspNetCore.Components.Web.HtmlRenderer>();
        services.TryAddScoped<IBlazorStaticRendererService, BlazorStaticRendererService>();

        return services;
    }
#endif

    /// <summary>
    ///     Adds services to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddBlazorRenderingContextService(this IServiceCollection services)
    {
        services.TryAddScoped<IBlazorRenderingContext, BlazorRenderingContext>();

        return services;
    }
}