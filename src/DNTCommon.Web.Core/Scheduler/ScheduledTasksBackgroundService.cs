using Microsoft.Extensions.Hosting;

namespace DNTCommon.Web.Core;

/// <summary>
///     ScheduledTasks Background Service
/// </summary>
public class ScheduledTasksBackgroundService(IScheduledTasksCoordinator scheduledTasksCoordinator) : BackgroundService
{
    /// <summary>
    ///     This method is called when the IHostedService starts.
    /// </summary>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        scheduledTasksCoordinator.StartTasks();

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
        => await scheduledTasksCoordinator.StopTasks();
}