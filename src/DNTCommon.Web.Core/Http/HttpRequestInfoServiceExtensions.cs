using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DNTCommon.Web.Core;

/// <summary>
/// Http Request Info Extensions
/// </summary>
public static class HttpRequestInfoServiceExtensions
{
    /// <summary>
    /// Adds IHttpContextAccessor, IActionContextAccessor, IUrlHelper and IHttpRequestInfoService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddHttpRequestInfoService(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
        // Allows injecting IUrlHelper as a dependency
        services.AddScoped(serviceProvider =>
        {
            var actionContext = serviceProvider.GetRequiredService<IActionContextAccessor>().ActionContext;
            var urlHelperFactory = serviceProvider.GetRequiredService<IUrlHelperFactory>();
            if (actionContext == null)
            {
                throw new InvalidOperationException("actionContext is nul. This code should be called within the MVC pipeline.");
            }
            return urlHelperFactory.GetUrlHelper(actionContext);
        });
        services.AddScoped<IHttpRequestInfoService, HttpRequestInfoService>();
        return services;
    }
}