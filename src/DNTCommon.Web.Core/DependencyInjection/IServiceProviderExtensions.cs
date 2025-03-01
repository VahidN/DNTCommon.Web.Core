using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     IServiceProvider Extensions
/// </summary>
public static class IServiceProviderExtensions
{
    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static void RunScopedService<TS, T>(this IServiceProvider serviceProvider, Action<TS, T> callback)
        where TS : notnull
        where T : notnull
    {
        using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        RunScopedService(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static T RunScopedService<TS, T>(this IServiceScope serviceScope, Func<TS, T> callback)
        where TS : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScope);

        ArgumentNullException.ThrowIfNull(callback);

        var context = serviceScope.ServiceProvider.GetRequiredService<TS>();

        return callback(context);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static T RunScopedService<TS, T>(this IServiceScopeFactory serviceScopeFactory, Func<TS, T> callback)
        where TS : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScopeFactory);

        using var serviceScope = serviceScopeFactory.CreateScope();

        return RunScopedService(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static T RunScopedService<TS, T>(this IServiceProvider serviceProvider, Func<TS, T> callback)
        where TS : notnull
    {
        using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

        return RunScopedService(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static void RunScopedService<TS>(this IServiceScope serviceScope, Action<TS> callback)
        where TS : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScope);

        ArgumentNullException.ThrowIfNull(callback);

        var context = serviceScope.ServiceProvider.GetRequiredService<TS>();

        try
        {
            callback(context);
        }
        finally
        {
            if (context is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static void RunScopedService<TS>(this IServiceScopeFactory serviceScopeFactory, Action<TS> callback)
        where TS : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScopeFactory);

        using var serviceScope = serviceScopeFactory.CreateScope();
        RunScopedService(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static void RunScopedService<TS>(this IServiceProvider serviceProvider, Action<TS> callback)
        where TS : notnull
    {
        using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        RunScopedService(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static void RunScopedService<TS, T>(this IServiceScope serviceScope, Action<TS, T> callback)
        where TS : notnull
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScope);

        ArgumentNullException.ThrowIfNull(callback);

        var context = serviceScope.ServiceProvider.GetRequiredService<TS>();

        try
        {
            callback(context, serviceScope.ServiceProvider.GetRequiredService<T>());
        }
        finally
        {
            if (context is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static void RunScopedService<TS, T>(this IServiceScopeFactory serviceScopeFactory, Action<TS, T> callback)
        where TS : notnull
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScopeFactory);

        using var serviceScope = serviceScopeFactory.CreateScope();
        RunScopedService(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static async Task RunScopedServiceAsync<TS, T>(this IServiceProvider serviceProvider,
        Func<TS, T, Task> callback)
        where TS : notnull
        where T : notnull
    {
        using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        await RunScopedServiceAsync(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static async Task RunScopedServiceAsync<TS, T>(this IServiceScopeFactory serviceScopeFactory,
        Func<TS, T, Task> callback)
        where TS : notnull
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScopeFactory);

        using var serviceScope = serviceScopeFactory.CreateScope();
        await RunScopedServiceAsync(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static async Task RunScopedServiceAsync<TS, T>(this IServiceScope serviceScope, Func<TS, T, Task> callback)
        where TS : notnull
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScope);

        ArgumentNullException.ThrowIfNull(callback);

        var context = serviceScope.ServiceProvider.GetRequiredService<TS>();

        try
        {
            await callback(context, serviceScope.ServiceProvider.GetRequiredService<T>());
        }
        finally
        {
            if (context is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static async Task RunScopedServiceAsync<TS>(this IServiceProvider serviceProvider, Func<TS, Task> callback)
        where TS : notnull
    {
        using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        await RunScopedServiceAsync(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static async Task RunScopedServiceAsync<TS>(this IServiceScopeFactory serviceScopeFactory,
        Func<TS, Task> callback)
        where TS : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScopeFactory);

        using var serviceScope = serviceScopeFactory.CreateScope();
        await RunScopedServiceAsync(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static async Task RunScopedServiceAsync<TS>(this IServiceScope serviceScope, Func<TS, Task> callback)
        where TS : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScope);

        ArgumentNullException.ThrowIfNull(callback);

        var context = serviceScope.ServiceProvider.GetRequiredService<TS>();

        try
        {
            await callback(context);
        }
        finally
        {
            if (context is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static async Task<T> RunScopedServiceAsync<TS, T>(this IServiceProvider serviceProvider,
        Func<TS, Task<T>> callback)
        where TS : notnull
    {
        using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

        return await RunScopedServiceAsync(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static async Task<T> RunScopedServiceAsync<TS, T>(this IServiceScopeFactory serviceScopeFactory,
        Func<TS, Task<T>> callback)
        where TS : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScopeFactory);

        using var serviceScope = serviceScopeFactory.CreateScope();

        return await RunScopedServiceAsync(serviceScope, callback);
    }

    /// <summary>
    ///     Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope
    ///     and then runs an associated callback.
    /// </summary>
    public static async Task<T> RunScopedServiceAsync<TS, T>(this IServiceScope serviceScope,
        Func<TS, Task<T>> callback)
        where TS : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceScope);

        ArgumentNullException.ThrowIfNull(callback);

        var context = serviceScope.ServiceProvider.GetRequiredService<TS>();

        return await callback(context);
    }
}
