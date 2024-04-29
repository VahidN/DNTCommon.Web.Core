using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DNTCommon.Web.Core;

/// <summary>
///     More info: http://www.dntips.ir/post/2564
/// </summary>
public static class RazorViewToStringRendererExtensions
{
    /// <summary>
    ///     Adds IViewRendererService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddRazorViewRenderer(this IServiceCollection services)
    {
        services.AddMvcCore().AddRazorViewEngine();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IViewRendererService, ViewRendererService>();

        return services;
    }
}