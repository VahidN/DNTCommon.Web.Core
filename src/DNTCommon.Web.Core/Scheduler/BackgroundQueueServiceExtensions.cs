using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Adds IQueueBackgroundWorkItem to IServiceCollection.
    /// </summary>
    public static class BackgroundQueueServiceExtensions
    {
        /// <summary>
        /// Used to register <see cref="IHostedService"/> class which defines an referenced <typeparamref name="TInterface"/> interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface other components will use</typeparam>
        /// <typeparam name="TService">The actual <see cref="IHostedService"/> service.</typeparam>
        /// <param name="services"></param>
        public static void AddHostedApiService<TInterface, TService>(this IServiceCollection services)
            where TInterface : class
            where TService : class, IHostedService, TInterface
        {
            services.AddSingleton<TInterface, TService>();
            services.AddHostedService(p => (TService)p.GetRequiredService<TInterface>());
        }

        /// <summary>
        /// Adds IQueueBackgroundWorkItem to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddBackgroundQueueService(this IServiceCollection services)
        {
            services.TryAddTransient<IJobsRunnerTimer, JobsRunnerTimer>();
            services.AddHostedApiService<IBackgroundQueueService, BackgroundQueueService>();
            return services;
        }
    }
}