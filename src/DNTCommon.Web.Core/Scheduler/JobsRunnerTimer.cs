using AsyncKeyedLock;
using ThreadTimer = System.Threading.Timer;

namespace DNTCommon.Web.Core;

/// <summary>
///     Periodically invokes OnTimerCallback logic,
///     and allows this periodic callback to be stopped in a thread-safe way.
/// </summary>
public class JobsRunnerTimer : IJobsRunnerTimer
{
    // <summary>
    // Used to control multi-threaded accesses to this instance
    // </summary>
    private readonly AsyncKeyedLocker<Type> _lock = new(new AsyncKeyedLockOptions());

    private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(value: 5);
    private bool _isDisposed;

    private ThreadTimer? _threadTimer; //keep it alive

    /// <summary>
    ///     Is timer alive?
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    ///     The periodic callback. Executing a method on a thread pool thread at specified intervals.
    /// </summary>
    public Action? OnThreadPoolTimerCallback { set; get; }

    /// <summary>
    ///     Starts the timer.
    /// </summary>
    public void StartJobs(int startAfter = 1000, int interval = 1000)
    {
        using var @lock = _lock.LockOrNull(typeof(JobsRunnerTimer), _lockTimeout);

        if (_threadTimer is not null)
        {
            return;
        }

        _threadTimer = new ThreadTimer(TimerCallback, state: null, Timeout.Infinite, period: 1000);
        _threadTimer.Change(startAfter, interval);
        IsRunning = true;
    }

    /// <summary>
    ///     Permanently stops the timer.
    /// </summary>
    public void StopJobs()
    {
        using var @lock = _lock.LockOrNull(typeof(JobsRunnerTimer), _lockTimeout);

        if (_threadTimer is null)
        {
            return;
        }

        _threadTimer.Dispose();
        _threadTimer = null;
        IsRunning = false;
    }

    /// <summary>
    ///     Free resources
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void TimerCallback(object? state) => OnThreadPoolTimerCallback?.Invoke();

    /// <summary>
    ///     Dispose all of the httpClients
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            try
            {
                if (disposing)
                {
                    StopJobs();
                    _lock.Dispose();
                }
            }
            finally
            {
                _isDisposed = true;
            }
        }
    }
}
