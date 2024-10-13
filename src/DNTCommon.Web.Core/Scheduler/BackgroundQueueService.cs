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
public class BackgroundQueueService(ILogger<BackgroundQueueService> logger, IServiceScopeFactory serviceScopeFactory)
    : BackgroundService, IBackgroundQueueService
{
    private readonly BlockingCollection<Func<CancellationToken, IServiceProvider, Task>> _asyncTasksQueue =
        new(new ConcurrentQueue<Func<CancellationToken, IServiceProvider, Task>>());

    private readonly TimeSpan _interval = TimeSpan.FromSeconds(value: 0.5);

    private readonly ILogger<BackgroundQueueService>
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly IServiceScopeFactory _serviceScopeFactory =
        serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

    private readonly BlockingCollection<Action<CancellationToken, IServiceProvider>> _syncTasksQueue =
        new(new ConcurrentQueue<Action<CancellationToken, IServiceProvider>>());

    private bool _isDisposed;

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
        _logger.LogDebug(message: "Background Queue Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_asyncTasksQueue.TryTake(out var asyncWorkItem))
                {
                    using var serviceScope = _serviceScopeFactory.CreateScope();
                    await asyncWorkItem(stoppingToken, serviceScope.ServiceProvider);
                }

                if (_syncTasksQueue.TryTake(out var workItem))
                {
                    using var serviceScope = _serviceScopeFactory.CreateScope();
                    workItem(stoppingToken, serviceScope.ServiceProvider);
                }

                await Task.Delay(_interval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Demystify(), message: "An error occurred executing the background job.");
            }
        }
    }

    /// <summary>
    ///     Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Stop();

        return Task.CompletedTask;
    }

    private void Stop()
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
            if (!disposing)
            {
                return;
            }

            Stop();
            _asyncTasksQueue.Dispose();
            _syncTasksQueue.Dispose();
        }
        finally
        {
            _isDisposed = true;
        }

        base.Dispose();
    }
}