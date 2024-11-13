using Microsoft.Extensions.Hosting;

namespace DNTCommon.Web.Core;

/// <summary>
///     ScheduledTasks Background Service
/// </summary>
/// <remarks>
///     ScheduledTasks Background Service
/// </remarks>
public class ScheduledTasksBackgroundService(IScheduledTasksCoordinator scheduledTasksCoordinator) : BackgroundService
{
    private readonly IScheduledTasksCoordinator _scheduledTasksCoordinator = scheduledTasksCoordinator ??
                                                                             throw new ArgumentNullException(
                                                                                 nameof(scheduledTasksCoordinator));

    /// <summary>
    ///     This method is called when the IHostedService starts.
    /// </summary>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _scheduledTasksCoordinator.StartTasks();

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
        => await _scheduledTasksCoordinator.StopTasks();
}