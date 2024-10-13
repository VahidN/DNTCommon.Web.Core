namespace DNTCommon.Web.Core;

/// <summary>
///     A .NET Core replacement for the old HostingEnvironment.QueueBackgroundWorkItem.
/// </summary>
public interface IBackgroundQueueService : IDisposable
{
    /// <summary>
    ///     Schedules a task which can run in the background, independent of any request.
    ///     The provided IServiceProvider is created from an IServiceScope.
    /// </summary>
    void QueueBackgroundWorkItem(Func<CancellationToken, IServiceProvider, Task> workItem);

    /// <summary>
    ///     Schedules a task which can run in the background, independent of any request.
    ///     The provided IServiceProvider is created from an IServiceScope.
    /// </summary>
    void QueueBackgroundWorkItem(Action<CancellationToken, IServiceProvider> workItem);
}