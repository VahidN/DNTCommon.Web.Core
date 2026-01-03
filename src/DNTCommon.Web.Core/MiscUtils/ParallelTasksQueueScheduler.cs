using System.Collections.Concurrent;

namespace DNTCommon.Web.Core;

/// <summary>
///     Represents an object that handles the low-level work of queuing tasks onto threads.
/// </summary>
public class ParallelTasksQueueScheduler : TaskScheduler, IDisposable
{
    private readonly BlockingCollection<Task> _tasks;
    private readonly ThreadPriority _threadPriority;
    private readonly List<Thread> _threads;
    private bool _disposed;

    /// <summary>
    ///     Represents an object that handles the low-level work of queuing tasks onto threads.
    /// </summary>
    /// <param name="numberOfThreads"></param>
    /// <param name="threadPriority"></param>
    public ParallelTasksQueueScheduler(int numberOfThreads, ThreadPriority threadPriority)
    {
        _threadPriority = threadPriority;

        if (numberOfThreads < 1)
        {
            numberOfThreads = Environment.ProcessorCount;
        }

        _tasks = [];

        _threads =
        [
            .. Enumerable.Range(start: 0, numberOfThreads)
                .Select(_ =>
                {
                    var thread = new Thread(() =>
                    {
                        foreach (var task in _tasks.GetConsumingEnumerable())
                        {
                            TryExecuteTask(task);
                        }
                    })
                    {
                        IsBackground = true,
                        Priority = _threadPriority
                    };

                    return thread;
                })
        ];

        _threads.ForEach(t => t.Start());
    }

    /// <summary>
    ///     Represents an object that handles the low-level work of queuing tasks onto threads.
    /// </summary>
    /// <param name="numberOfThreads"></param>
    public ParallelTasksQueueScheduler(int numberOfThreads) : this(numberOfThreads, ThreadPriority.Normal)
    {
    }

    /// <inheritdoc />
    public override int MaximumConcurrencyLevel => _threads.Count;

    /// <summary>
    ///     Free resources
    /// </summary>
    public void Dispose()
    {
        Dispose(disposeManagedResources: true);

        // tell the GC that the Finalize process no longer needs to be run for this object.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Free resources
    /// </summary>
    protected virtual void Dispose(bool disposeManagedResources)
    {
        if (_disposed)
        {
            return;
        }

        if (!disposeManagedResources)
        {
            return;
        }

        try
        {
            _tasks.CompleteAdding();

            foreach (var thread in _threads)
            {
                thread.Join();
            }

            _tasks.Dispose();
        }
        finally
        {
            _disposed = true;
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<Task> GetScheduledTasks() => _tasks.ToArray();

    /// <inheritdoc />
    protected override void QueueTask(Task task) => _tasks.Add(task);

    /// <inheritdoc />
    protected override bool TryDequeue(Task task) => base.TryDequeue(task);

    /// <inheritdoc />
    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        => Thread.CurrentThread.Priority == _threadPriority && TryExecuteTask(task);
}
