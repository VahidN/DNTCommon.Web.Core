using System;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// IServiceProvider Extensions
    /// </summary>
    public static class IServiceProviderExtensions
    {
        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScopedService<T, S>(this IServiceProvider serviceProvider, Action<S, T> callback)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                RunScopedService(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScopedService<T, S>(this IServiceScopeFactory serviceScopeFactory, Action<S, T> callback)
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                RunScopedService(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScopedService<T, S>(this IServiceScope serviceScope, Action<S, T> callback)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<S>();
            callback(context, serviceScope.ServiceProvider.GetRequiredService<T>());
            if (context is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScopedService<S>(this IServiceProvider serviceProvider, Action<S> callback)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                RunScopedService(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScopedService<S>(this IServiceScopeFactory serviceScopeFactory, Action<S> callback)
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                RunScopedService(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScopedService<S>(this IServiceScope serviceScope, Action<S> callback)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<S>();
            callback(context);
            if (context is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static T RunScopedService<T, S>(this IServiceProvider serviceProvider, Func<S, T> callback)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                return RunScopedService(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static T RunScopedService<T, S>(this IServiceScopeFactory serviceScopeFactory, Func<S, T> callback)
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                return RunScopedService(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static T RunScopedService<T, S>(this IServiceScope serviceScope, Func<S, T> callback)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<S>();
            return callback(context);
        }
    }
}