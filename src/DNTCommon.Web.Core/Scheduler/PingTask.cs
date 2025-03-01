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
    ///     Scheduled task's logic.
    /// </summary>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await pingClient.WakeUpAsync();
    }
}
