using System;
using System.Threading.Tasks;
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
        public static void RunScopedService<S, T>(this IServiceProvider serviceProvider, Action<S, T> callback)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                RunScopedService(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static async Task RunScopedServiceAsync<S, T>(this IServiceProvider serviceProvider, Func<S, T, Task> callback)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                await RunScopedServiceAsync(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScopedService<S, T>(this IServiceScopeFactory serviceScopeFactory, Action<S, T> callback)
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                RunScopedService(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static async Task RunScopedServiceAsync<S, T>(this IServiceScopeFactory serviceScopeFactory, Func<S, T, Task> callback)
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                await RunScopedServiceAsync(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScopedService<S, T>(this IServiceScope serviceScope, Action<S, T> callback)
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
        public static async Task RunScopedServiceAsync<S, T>(this IServiceScope serviceScope, Func<S, T, Task> callback)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<S>();
            await callback(context, serviceScope.ServiceProvider.GetRequiredService<T>());
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
        public static async Task RunScopedServiceAsync<S>(this IServiceProvider serviceProvider, Func<S, Task> callback)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                await RunScopedServiceAsync(serviceScope, callback);
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
        public static async Task RunScopedServiceAsync<S>(this IServiceScopeFactory serviceScopeFactory, Func<S, Task> callback)
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                await RunScopedServiceAsync(serviceScope, callback);
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
        public static async Task RunScopedServiceAsync<S>(this IServiceScope serviceScope, Func<S, Task> callback)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<S>();
            await callback(context);
            if (context is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static T RunScopedService<S, T>(this IServiceProvider serviceProvider, Func<S, T> callback)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                return RunScopedService(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static async Task<T> RunScopedServiceAsync<S, T>(this IServiceProvider serviceProvider, Func<S, Task<T>> callback)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                return await RunScopedServiceAsync(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static T RunScopedService<S, T>(this IServiceScopeFactory serviceScopeFactory, Func<S, T> callback)
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                return RunScopedService(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static async Task<T> RunScopedServiceAsync<S, T>(this IServiceScopeFactory serviceScopeFactory, Func<S, Task<T>> callback)
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                return await RunScopedServiceAsync(serviceScope, callback);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static T RunScopedService<S, T>(this IServiceScope serviceScope, Func<S, T> callback)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<S>();
            return callback(context);
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static async Task<T> RunScopedServiceAsync<S, T>(this IServiceScope serviceScope, Func<S, Task<T>> callback)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<S>();
            return await callback(context);
        }
    }
}