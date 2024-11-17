using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DNTCommon.Web.Core;

/// <summary>
///     Http Request Info Extensions
/// </summary>
public static class HttpRequestInfoServiceExtensions
{
    /// <summary>
    ///     Adds IHttpContextAccessor, IActionContextAccessor, IUrlHelper and IHttpRequestInfoService to IServiceCollection.
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

            if (actionContext != null)
            {
                return urlHelperFactory.GetUrlHelper(actionContext);
            }

            return urlHelperFactory.GetUrlHelper(new ActionContext(new DefaultHttpContext
            {
                RequestServices = serviceProvider
            }, new RouteData(), new ActionDescriptor()));
        });

        services.AddScoped<IHttpRequestInfoService, HttpRequestInfoService>();

        return services;
    }

    /// <summary>
    ///     Configures IISServerOptions to support uploading very large files.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddLargeFilesUploadSupport(this IServiceCollection services)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = int.MaxValue; });
            }
        }
        catch (Exception ex)
        {
            WriteLine(ex);
        }

        services.Configure<FormOptions>(options =>
        {
            options.ValueLengthLimit = int.MaxValue;
            options.MultipartBodyLengthLimit = long.MaxValue;
            options.MultipartBoundaryLengthLimit = int.MaxValue;
            options.MultipartHeadersCountLimit = int.MaxValue;
            options.MultipartHeadersLengthLimit = int.MaxValue;
        });

        return services;
    }
}