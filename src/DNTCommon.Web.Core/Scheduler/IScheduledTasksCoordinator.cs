namespace DNTCommon.Web.Core;

/// <summary>
///     Scheduled Tasks Manager
/// </summary>
public interface IScheduledTasksCoordinator : IDisposable
{
    /// <summary>
    ///     Starts the scheduler.
    /// </summary>
    void StartTasks(CancellationToken stoppingToken = default);

    /// <summary>
    ///     Stops the scheduler.
    /// </summary>
    Task StopTasksAsync();
}
