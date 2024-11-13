using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

/// <summary>
///     DNTScheduler ServiceCollection Extensions
/// </summary>
public static class SchedulerServiceCollectionExtensions
{
    /// <summary>
    ///     Adds default DNTScheduler providers.
    /// </summary>
    public static void AddDNTScheduler(this IServiceCollection services, Action<ScheduledTasksStorage> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        services.TryAddSingleton<IScheduledTasksCoordinator, ScheduledTasksCoordinator>();

        ConfigTasks(services, options);
    }

    private static void ConfigTasks(IServiceCollection services, Action<ScheduledTasksStorage> options)
    {
        var storage = new ScheduledTasksStorage();
        options(storage);
        RegisterTasks(services, storage);
        AddPingTask(services, storage);
        services.TryAddSingleton(Options.Create(storage));
    }

    private static void RegisterTasks(IServiceCollection services, ScheduledTasksStorage storage)
    {
        foreach (var task in storage.Tasks)
        {
            services.TryAddTransient(task.TaskType);
        }

        services.AddHostedService<ScheduledTasksBackgroundService>();
    }

    private static void AddPingTask(IServiceCollection services, ScheduledTasksStorage storage)
    {
        if (string.IsNullOrWhiteSpace(storage.SiteRootUrl))
        {
            return;
        }

        services.AddHttpClient<MySitePingClient>(client =>
            {
                client.BaseAddress = new Uri(storage.SiteRootUrl);
                client.DefaultRequestHeaders.ConnectionClose = true;
                client.DefaultRequestHeaders.Add(name: "User-Agent", value: "DNTScheduler 1.0");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All
            });

        storage.AddScheduledTask<PingTask>(utcNow => utcNow.Second == 1);
        services.TryAddSingleton<PingTask>();
    }
}