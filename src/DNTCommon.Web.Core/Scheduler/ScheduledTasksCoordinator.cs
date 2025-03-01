using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

/// <summary>
///     Scheduled Tasks Manager
/// </summary>
public sealed class ScheduledTasksCoordinator : IScheduledTasksCoordinator
{
    // the 30 seconds is for the entire app to tie up what it's doing.
    private const int TimeToFinish = 30 * 1000;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly IJobsRunnerTimer _jobsRunnerTimer;
    private readonly ILogger<ScheduledTasksCoordinator> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<ScheduledTasksStorage> _tasksStorage;

    private bool _isDisposed;
    private bool _isShuttingDown;

    /// <summary>
    ///     Scheduled Tasks Manager
    /// </summary>
    public ScheduledTasksCoordinator(ILogger<ScheduledTasksCoordinator> logger,
        IHostApplicationLifetime applicationLifetime,
        IOptions<ScheduledTasksStorage> tasksStorage,
        IJobsRunnerTimer jobsRunnerTimer,
        IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tasksStorage = tasksStorage ?? throw new ArgumentNullException(nameof(tasksStorage));
        _jobsRunnerTimer = jobsRunnerTimer ?? throw new ArgumentNullException(nameof(jobsRunnerTimer));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        ArgumentNullException.ThrowIfNull(applicationLifetime);

        applicationLifetime.ApplicationStopping.Register(() =>
        {
            _logger.LogWarning(message: "Application is stopping ... .");
            DisposeResourcesAsync().Wait();
        });
    }

    /// <summary>
    ///     Starts the scheduler.
    /// </summary>
    public void StartTasks()
    {
        if (_jobsRunnerTimer.IsRunning)
        {
            return;
        }

        _jobsRunnerTimer.OnThreadPoolTimerCallback = () =>
        {
            var now = DateTime.UtcNow;

            var tasks = new List<Task>();

            foreach (var taskStatus in _tasksStorage.Value.Tasks.Where(x => x.RunAt(now)).OrderBy(x => x.Order))
            {
                if (_isShuttingDown)
                {
                    return;
                }

                if (taskStatus.IsRunning)
                {
                    _logger.LogInformation(message: "Ignoring `{TaskStatus}` task. It's still running.", taskStatus);

                    continue;
                }

                tasks.Add(Task.Run(() => RunTask(taskStatus, now)));
            }

            if (tasks.Count != 0)
            {
                Task.WaitAll([.. tasks]);
            }
        };

        _jobsRunnerTimer.StartJobs();
    }

    /// <summary>
    ///     Stops the scheduler.
    /// </summary>
    public Task StopTasksAsync() => DisposeResourcesAsync();

    /// <summary>
    ///     Free resources
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private async Task DisposeResourcesAsync()
    {
        if (_isShuttingDown || _isDisposed)
        {
            return;
        }

        try
        {
            _isShuttingDown = true;

#if NET_6 || NET_7
            _cancellationTokenSource.Cancel();
#else
            await _cancellationTokenSource.CancelAsync();
#endif

            var timeOut = TimeToFinish;

            while (_tasksStorage.Value.Tasks.Any(x => x.IsRunning) && timeOut >= 0)
            {
                // still running ...
                await Task.Delay(millisecondsDelay: 50);
                timeOut -= 50;
            }
        }
        finally
        {
            _jobsRunnerTimer.StopJobs();
            _cancellationTokenSource.Dispose();
            _isDisposed = true;
            await WakeUpAsync();
        }
    }

    private async Task WakeUpAsync()
    {
        var mySitePingClient = _serviceProvider.GetService<MySitePingClient>();

        if (mySitePingClient is not null)
        {
            await mySitePingClient.WakeUpAsync();
        }
    }

    private void RunTask(ScheduledTaskStatus taskStatus, DateTime now)
    {
        var watch = new Stopwatch();
        watch.Start();

        var name = taskStatus.TaskType.FullName;

        try
        {
            using var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var scheduledTask = (IScheduledTask)serviceScope.ServiceProvider.GetRequiredService(taskStatus.TaskType);

            taskStatus.TaskInstance = scheduledTask;

            if (_isShuttingDown)
            {
                return;
            }

            taskStatus.IsRunning = true;
            taskStatus.LastRun = now;

            _logger.LogInformation(message: "Start running `{Name}` task @ {Now}.", name, now);
            scheduledTask.RunAsync(_cancellationTokenSource.Token).Wait();

            _logger.LogInformation(message: "Finished running `{Name}` task @ {Now}.", name, now);
            taskStatus.IsLastRunSuccessful = true;
        }
        catch (Exception ex)
        {
            var exception = ex.Demystify();

            _logger.LogCritical(eventId: 0, exception, message: "Failed running `{Name}` after `{Time}`.", name,
                watch.Elapsed);

            taskStatus.IsLastRunSuccessful = false;
            taskStatus.LastException = exception;
        }
        finally
        {
            watch.Stop();
            taskStatus.IsRunning = false;
            taskStatus.TaskInstance = null;
        }
    }

    /// <summary>
    ///     Free resources
    /// </summary>
    private void Dispose(bool disposing)
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

            StopTasksAsync().Wait();
        }
        finally
        {
            _isDisposed = true;
        }
    }
}
