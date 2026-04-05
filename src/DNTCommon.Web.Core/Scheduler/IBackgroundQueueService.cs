namespace DNTCommon.Web.Core;

/// <summary>
///     A .NET Core replacement for the old HostingEnvironment.QueueBackgroundWorkItem.
/// </summary>
public interface IBackgroundQueueService : IDisposable
{
    /// <summary>
    ///     Schedules a task which can run in the background, independent of any request.
    ///     The provided IServiceProvider is created from an IServiceScope.
    ///     Usage: _queueService.QueueBackgroundWorkItemAsync(async (cancellationToken, serviceProvider) => {    });
    /// </summary>
    ValueTask QueueBackgroundWorkItemAsync(string group,
        Func<CancellationToken, IServiceProvider, Task>? workItem,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Schedules a task which can run in the background, independent of any request.
    ///     The provided IServiceProvider is created from an IServiceScope.
    ///     Usage: _queueService.QueueBackgroundWorkItemAsync((cancellationToken, serviceProvider) => {    });
    /// </summary>
    ValueTask QueueBackgroundWorkItemAsync(string group,
        Action<CancellationToken, IServiceProvider>? workItem,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Attempts to queue a background Task which can run in the background, independent of any request.
    ///     The provided IServiceProvider is created from an IServiceScope.
    /// </summary>
    bool TryQueueBackgroundWorkItem(string group, Func<CancellationToken, IServiceProvider, Task>? workItem);

    /// <summary>
    ///     Attempts to queue a background Task which can run in the background, independent of any request.
    ///     The provided IServiceProvider is created from an IServiceScope.
    /// </summary>
    bool TryQueueBackgroundWorkItem(string group, Action<CancellationToken, IServiceProvider>? workItem);
}
