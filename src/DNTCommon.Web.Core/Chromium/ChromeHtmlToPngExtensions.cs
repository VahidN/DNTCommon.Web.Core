using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Adds ChromeHtmlToPng Services to IServiceCollection.
/// </summary>
public static class ChromeHtmlToPngExtensions
{
    /// <summary>
    ///     Adds ChromeHtmlToPng Services to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddChromeHtmlToPngService(this IServiceCollection services)
    {
        services.AddSingleton<IHtmlToPngGenerator, ChromeHtmlToPngGenerator>();
        services.AddSingleton<IExecuteApplicationProcess, ExecuteApplicationProcess>();

        return services;
    }
}