using System;
using System.Threading;
using ThreadTimer = System.Threading.Timer;

namespace DNTCommon.Web.Core;

/// <summary>
/// Periodically invokes OnTimerCallback logic,
/// and allows this periodic callback to be stopped in a thread-safe way.
/// </summary>
public class JobsRunnerTimer : IJobsRunnerTimer
{
    private bool _isDisposed;

    // <summary>
    // Used to control multi-threaded accesses to this instance
    // </summary>
    private readonly object _sync = new object();

    private ThreadTimer? _threadTimer; //keep it alive

    /// <summary>
    /// Is timer alive?
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// The periodic callback. Executing a method on a thread pool thread at specified intervals.
    /// </summary>
    public Action? OnThreadPoolTimerCallback { set; get; }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    public void StartJobs(int startAfter = 1000, int interval = 1000)
    {
        lock (_sync)
        {
            if (_threadTimer != null)
            {
                return;
            }

            _threadTimer = new ThreadTimer(timerCallback, null, Timeout.Infinite, 1000);
            _threadTimer.Change(startAfter, interval);
            IsRunning = true;
        }
    }

    /// <summary>
    /// Permanently stops the timer.
    /// </summary>
    public void StopJobs()
    {
        lock (_sync)
        {
            if (_threadTimer == null)
            {
                return;
            }

            _threadTimer.Dispose();
            _threadTimer = null;
            IsRunning = false;
        }
    }

    private void timerCallback(object? state)
    {
        OnThreadPoolTimerCallback?.Invoke();
    }

    /// <summary>
    /// Free resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose all of the httpClients
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
                }
            }
            finally
            {
                _isDisposed = true;
            }
        }
    }
}