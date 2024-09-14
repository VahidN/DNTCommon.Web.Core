using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     ErrorBoundaryLogger Service Extensions
/// </summary>
public static class ErrorBoundaryLoggerExtensions
{
    /// <summary>
    ///     Adds IErrorBoundaryLogger to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddErrorBoundaryLoggerService(this IServiceCollection services)
    {
        services.AddScoped<IErrorBoundaryLogger, ErrorBoundaryLoggingService>();

        return services;
    }
}