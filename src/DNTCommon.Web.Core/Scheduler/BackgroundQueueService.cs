using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     BackgroundQueue Service
///     A .NET Core replacement for the old HostingEnvironment.QueueBackgroundWorkItem.
/// </summary>
/// <remarks>
///     BackgroundQueue Service
/// </remarks>
public class BackgroundQueueService(IServiceProvider serviceProvider) : BackgroundService, IBackgroundQueueService
{
    private readonly BlockingCollection<Func<CancellationToken, IServiceProvider, Task>> _asyncTasksQueue =
        new(new ConcurrentQueue<Func<CancellationToken, IServiceProvider, Task>>());

    private readonly TimeSpan _interval = TimeSpan.FromSeconds(value: 0.5);

    private readonly BlockingCollection<Action<CancellationToken, IServiceProvider>> _syncTasksQueue =
        new(new ConcurrentQueue<Action<CancellationToken, IServiceProvider>>());

    private bool _isDisposed;

    // we want to prevent a circular dependency between ILoggerFactory and CustomLoggers
    private ILoggerFactory? _loggerFactory;

    private ILoggerFactory LoggerFactory => _loggerFactory ??= serviceProvider.GetRequiredService<ILoggerFactory>();

    /// <summary>
    ///     Queues a background Task
    /// </summary>
    public void QueueBackgroundWorkItem(Func<CancellationToken, IServiceProvider, Task> workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);

        if (!_asyncTasksQueue.IsAddingCompleted)
        {
            _asyncTasksQueue.Add(workItem);
        }
    }

    /// <summary>
    ///     Queues a background Task
    /// </summary>
    public void QueueBackgroundWorkItem(Action<CancellationToken, IServiceProvider> workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);

        if (!_syncTasksQueue.IsAddingCompleted)
        {
            _syncTasksQueue.Add(workItem);
        }
    }

    /// <summary>
    ///     Free resources
    /// </summary>
    public new void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     This method is called when the IHostedService starts.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var logger = LoggerFactory.CreateLogger<BackgroundQueueService>();
        logger.LogDebug(message: "Background Queue Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_asyncTasksQueue.TryTake(out var asyncWorkItem, millisecondsTimeout: 0, stoppingToken))
                {
                    await using var serviceScope = GetServiceScopeAsync();
                    await asyncWorkItem(stoppingToken, serviceScope.ServiceProvider);
                }

                if (_syncTasksQueue.TryTake(out var workItem, millisecondsTimeout: 0, stoppingToken))
                {
                    await using var serviceScope = GetServiceScopeAsync();
                    workItem(stoppingToken, serviceScope.ServiceProvider);
                }

                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning(ex.Demystify(),
                    message: "Graceful Shutdown. Cancellation has been requested for this task.");
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex.Demystify(), message: "Cancellation has been requested for this task.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Demystify(), message: "An error occurred executing the background job.");
            }
        }
    }

    private AsyncServiceScope GetServiceScopeAsync()
        => serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();

    /// <summary>
    ///     Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        StopQueue();

        return Task.CompletedTask;
    }

    private void StopQueue()
    {
        _asyncTasksQueue.CompleteAdding();
        _syncTasksQueue.CompleteAdding();
    }

    /// <summary>
    ///     Free resources
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        try
        {
            if (disposing)
            {
                StopQueue();
                _asyncTasksQueue.Dispose();
                _syncTasksQueue.Dispose();
            }
        }
        finally
        {
            _isDisposed = true;
        }

        base.Dispose();
    }
}
