namespace DNTCommon.Web.Core;

/// <summary>
///     DNTScheduler needs a ping service to keep it alive.
/// </summary>
/// <remarks>
///     DNTScheduler needs a ping service to keep it alive.
/// </remarks>
public class PingTask(MySitePingClient pingClient) : IScheduledTask
{
    /// <summary>
    ///     Is ASP.Net app domain tearing down?
    ///     If set to true by the coordinator, the task should cleanup and return.
    /// </summary>
    public bool IsShuttingDown { get; set; }

    /// <summary>
    ///     Scheduled task's logic.
    /// </summary>
    public async Task RunAsync()
    {
        if (IsShuttingDown)
        {
            return;
        }

        await pingClient.WakeUp();
    }
}