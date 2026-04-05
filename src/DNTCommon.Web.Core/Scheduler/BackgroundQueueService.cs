using System.Threading.Channels;
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
    private const int ChannelCapacity = 1000;

    private static readonly BoundedChannelOptions BoundedChannelOptions = new(ChannelCapacity)
    {
        FullMode = BoundedChannelFullMode.Wait,
        SingleReader = true,
        SingleWriter = false
    };

    private readonly ConcurrentDictionaryLocked<string, Channel<Func<CancellationToken, IServiceProvider, Task>>>
        _asyncTasksChannels = new(StringComparer.OrdinalIgnoreCase);

    private readonly TimeSpan _interval = TimeSpan.FromMilliseconds(value: 10);

    private readonly ConcurrentDictionaryLocked<string, Channel<Action<CancellationToken, IServiceProvider>>>
        _syncTasksChannels = new(StringComparer.OrdinalIgnoreCase);

    private bool _isDisposed;

    // we want to prevent a circular dependency between ILoggerFactory and CustomLoggers
    private ILoggerFactory? _loggerFactory;

    private ILoggerFactory LoggerFactory => _loggerFactory ??= serviceProvider.GetRequiredService<ILoggerFactory>();

    /// <summary>
    ///     Queues a background Task
    /// </summary>
    public ValueTask QueueBackgroundWorkItemAsync(string group,
        Func<CancellationToken, IServiceProvider, Task>? workItem,
        CancellationToken cancellationToken = default)
    {
        if (workItem is null)
        {
            return ValueTask.CompletedTask;
        }

        var channel = _asyncTasksChannels.LockedGetOrAdd(group, _ => CreateAsyncTasksQueue());

        return channel.Writer.WriteAsync(workItem, cancellationToken);
    }

    /// <summary>
    ///     Attempts to queue a background Task
    /// </summary>
    public bool TryQueueBackgroundWorkItem(string group, Func<CancellationToken, IServiceProvider, Task>? workItem)
    {
        if (workItem is null)
        {
            return false;
        }

        var channel = _asyncTasksChannels.LockedGetOrAdd(group, _ => CreateAsyncTasksQueue());

        return channel.Writer.TryWrite(workItem);
    }

    /// <summary>
    ///     Queues a background Task
    /// </summary>
    public ValueTask QueueBackgroundWorkItemAsync(string group,
        Action<CancellationToken, IServiceProvider>? workItem,
        CancellationToken cancellationToken = default)
    {
        if (workItem is null)
        {
            return ValueTask.CompletedTask;
        }

        var channel = _syncTasksChannels.LockedGetOrAdd(group, _ => CreateSyncTasksQueue());

        return channel.Writer.WriteAsync(workItem, cancellationToken);
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
    ///     Attempts to queue a background Task
    /// </summary>
    public bool TryQueueBackgroundWorkItem(string group, Action<CancellationToken, IServiceProvider>? workItem)
    {
        if (workItem is null)
        {
            return false;
        }

        var channel = _syncTasksChannels.LockedGetOrAdd(group, _ => CreateSyncTasksQueue());

        return channel.Writer.TryWrite(workItem);
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
                await Task.WhenAll(ProcessAsyncTasksAsync(stoppingToken), ProcessSyncTasksAsync(stoppingToken),
                    Task.Delay(_interval, stoppingToken));
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
        foreach (var channel in GetAsyncTasksChannelsList())
        {
            channel.Writer.TryComplete();
        }

        foreach (var channel in GetSyncTasksChannelsList())
        {
            channel.Writer.TryComplete();
        }
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
            }
        }
        finally
        {
            _isDisposed = true;
        }

        base.Dispose();
    }

    private List<Channel<Func<CancellationToken, IServiceProvider, Task>>> GetAsyncTasksChannelsList()
        => [.. _asyncTasksChannels.Values.Select(c => c.Value)];

    private List<Channel<Action<CancellationToken, IServiceProvider>>> GetSyncTasksChannelsList()
        => [.._syncTasksChannels.Values.Select(c => c.Value)];

    private static Channel<Func<CancellationToken, IServiceProvider, Task>> CreateAsyncTasksQueue()
        => Channel.CreateBounded<Func<CancellationToken, IServiceProvider, Task>>(BoundedChannelOptions);

    private static Channel<Action<CancellationToken, IServiceProvider>> CreateSyncTasksQueue()
        => Channel.CreateBounded<Action<CancellationToken, IServiceProvider>>(BoundedChannelOptions);

    private AsyncServiceScope GetServiceScopeAsync()
        => serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();

#pragma warning disable CC001,CC002
    private async Task ProcessSyncTasksAsync(CancellationToken stoppingToken)
    {
        var syncTasksChannelsList = GetSyncTasksChannelsList();

        await Parallel.ForEachAsync(syncTasksChannelsList, new ParallelOptions
        {
            CancellationToken = stoppingToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, async (channel, ct) =>
        {
            while (!ct.IsCancellationRequested)
            {
                if (channel.Reader.TryRead(out var workItem))
                {
                    await using var serviceScope = GetServiceScopeAsync();
                    workItem(stoppingToken, serviceScope.ServiceProvider);
                }
                else
                {
                    break;
                }

                await Task.Delay(_interval, ct);
            }
        });
    }

    private async Task ProcessAsyncTasksAsync(CancellationToken stoppingToken)
    {
        var asyncTasksChannelsList = GetAsyncTasksChannelsList();

        await Parallel.ForEachAsync(asyncTasksChannelsList, new ParallelOptions
        {
            CancellationToken = stoppingToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, async (channel, ct) =>
        {
            while (!ct.IsCancellationRequested)
            {
                if (channel.Reader.TryRead(out var asyncWorkItem))
                {
                    await using var serviceScope = GetServiceScopeAsync();
                    await asyncWorkItem(ct, serviceScope.ServiceProvider);
                }
                else
                {
                    break;
                }

                await Task.Delay(_interval, ct);
            }
        });
    }
#pragma warning restore CC001,CC002
}
