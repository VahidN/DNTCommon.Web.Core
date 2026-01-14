using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     ICancellationContext Extensions
/// </summary>
public static class AsyncLocalCancellationContextExtensions
{
    /// <summary>
    ///     Adds ICancellationContext to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddAsyncLocalCancellationContext(this IServiceCollection services)
    {
        services.AddSingleton<IAsyncLocalCancellationContext, AsyncLocalCancellationContext>();

        return services;
    }
}
