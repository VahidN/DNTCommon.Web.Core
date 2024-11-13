namespace DNTCommon.Web.Core;

/// <summary>
///     Scheduled Tasks Manager
/// </summary>
public interface IScheduledTasksCoordinator
{
    /// <summary>
    ///     Starts the scheduler.
    /// </summary>
    void StartTasks();

    /// <summary>
    ///     Stops the scheduler.
    /// </summary>
    Task StopTasks();
}