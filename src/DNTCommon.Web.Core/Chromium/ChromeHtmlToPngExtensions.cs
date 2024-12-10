using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Adds ChromeHtmlToXYZ Services to IServiceCollection.
/// </summary>
public static class ChromeHtmlToPngExtensions
{
    /// <summary>
    ///     Adds ChromeHtmlToXYZ Services to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddChromeHtmlToPngService(this IServiceCollection services)
    {
        services.AddSingleton<IHtmlToPngGenerator, ChromeHtmlToPngGenerator>();
        services.AddSingleton<IHtmlToPdfGenerator, ChromeHtmlToPdfGenerator>();
        services.AddSingleton<IExecuteApplicationProcess, ExecuteApplicationProcess>();

        return services;
    }
}