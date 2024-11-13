namespace DNTCommon.Web.Core;

/// <summary>
///     Periodically invokes OnTimerCallback logic,
///     and allows this periodic callback to be stopped in a thread-safe way.
/// </summary>
public interface IJobsRunnerTimer : IDisposable
{
    /// <summary>
    ///     Is timer alive?
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    ///     The periodic callback. Executing a method on a thread pool thread at specified intervals.
    /// </summary>
    Action? OnThreadPoolTimerCallback { set; get; }

    /// <summary>
    ///     Starts the timer.
    /// </summary>
    void StartJobs(int startAfter = 1000, int interval = 1000);

    /// <summary>
    ///     Permanently stops the timer.
    /// </summary>
    void StopJobs();
}