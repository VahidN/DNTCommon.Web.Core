using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DNTCommon.Web.Core;

/// <summary>
///     AutoInject ServicesExtensions
/// </summary>
public static class AutoInjectServicesExtensions
{
    /// <summary>
    ///     Adds services which implement IScopedService, ISingletonService, IMiddleware, ITransientService and
    ///     BackgroundService to the service collection from the Assembly of the method that invoked the currently executing
    ///     method.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AutoInjectAllServices(this IServiceCollection services)
        => AutoInjectAllServices(services, Assembly.GetCallingAssembly());

    /// <summary>
    ///     Adds services which implement IScopedService, ISingletonService, IMiddleware, ITransientService and
    ///     BackgroundService to the service collection from the specified assemblies.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">The preferred services assemblies</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AutoInjectAllServices(this IServiceCollection services,
        params ICollection<Assembly> assemblies)
    {
        // Using the `Scrutor` to add all the application's services at once.
        services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<ISingletonService>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            .AddClasses(classes => classes.AssignableTo<BackgroundService>())
            .As<IHostedService>()
            .WithSingletonLifetime()
            .AddClasses(classes => classes.AssignableTo<IScopedService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ITransientService>())
            .AsImplementedInterfaces()
            .WithTransientLifetime()
            .AddClasses(classes => classes.Where(type =>
            {
                var allInterfaces = type.GetInterfaces();

                return allInterfaces.Contains(typeof(IMiddleware)) && allInterfaces.Contains(typeof(ISingletonService));
            }))
            .AsSelf()
            .WithSingletonLifetime());

        return services;
    }
}