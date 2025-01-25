namespace DNTCommon.Web.Core;

/// <summary>
///     Parallel Extensions
/// </summary>
public static class ParallelExtensions
{
    /// <summary>
    ///     Schedules a cancel operation on this CancellationTokenSource after the specified time span.
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static CancellationTokenSource ToCancellationTokenSource(this TimeSpan timeout)
    {
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(timeout);

        return cancellationTokenSource;
    }

    /// <summary>
    ///     Executes the provided action multiple times, possibly in parallel.
    /// </summary>
    public static void ExecuteInParallel(this Action test, int times, TimeSpan timeout)
    {
        using var cts = timeout.ToCancellationTokenSource();
        ExecuteInParallel(test, times, cts.Token);
    }

    /// <summary>
    ///     Executes the provided action multiple times, possibly in parallel.
    /// </summary>
    public static void ExecuteInParallel(this Action test, int times, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(test);

        var tests = new Action[times];

        for (var i = 0; i < times; i++)
        {
            tests[i] = test;
        }

        Parallel.Invoke(new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, tests);
    }

    /// <summary>
    ///     Executes the provided actions, possibly in parallel.
    /// </summary>
    public static void ExecuteInParallel(this ICollection<Action> actions, TimeSpan timeout)
    {
        using var cts = timeout.ToCancellationTokenSource();
        ExecuteInParallel(actions, cts.Token);
    }

    /// <summary>
    ///     Executes the provided actions, possibly in parallel.
    /// </summary>
    public static void ExecuteInParallel(this ICollection<Action> actions,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(actions);

        Parallel.Invoke(new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, [..actions]);
    }

    /// <summary>
    ///     Executes the provided actions, possibly in parallel.
    /// </summary>
    public static async Task ExecuteInParallelAsync<TParameter>(this ICollection<TParameter> parameters,
        Func<TParameter, CancellationToken, Task> action,
        TimeSpan timeout)
    {
        using var cts = timeout.ToCancellationTokenSource();

        await ExecuteInParallelAsync(parameters, action, cts.Token);
    }

    /// <summary>
    ///     Executes the provided actions, possibly in parallel.
    /// </summary>
    public static Task ExecuteInParallelAsync<TParameter>(this ICollection<TParameter> parameters,
        Func<TParameter, CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        return Parallel.ForEachAsync(parameters, new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, async (parameter, ct) => { await action(parameter, ct); });
    }

    /// <summary>
    ///     Executes the provided actions, possibly in parallel.
    /// </summary>
    public static async Task ExecuteInParallelAsync<TParameter>(this ICollection<TParameter> parameters,
        Func<TParameter, Task> action,
        TimeSpan timeout)
    {
        using var cts = timeout.ToCancellationTokenSource();
        await ExecuteInParallelAsync(parameters, action, cts.Token);
    }

    /// <summary>
    ///     Executes the provided actions, possibly in parallel.
    /// </summary>
    public static Task ExecuteInParallelAsync<TParameter>(this ICollection<TParameter> parameters,
        Func<TParameter, Task> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        return Parallel.ForEachAsync(parameters, new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, async (parameter, _) => { await action(parameter); });
    }

    /// <summary>
    ///     Executes the provided actions, possibly in parallel.
    /// </summary>
    public static async Task ExecuteInParallelAsync(this ICollection<Func<CancellationToken, Task>> actions,
        TimeSpan timeout)
    {
        using var cts = timeout.ToCancellationTokenSource();
        await ExecuteInParallelAsync(actions, cts.Token);
    }

    /// <summary>
    ///     Executes the provided actions, possibly in parallel.
    /// </summary>
    public static Task ExecuteInParallelAsync(this ICollection<Func<CancellationToken, Task>> actions,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(actions);

        return Parallel.ForEachAsync(actions, new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, async (action, ct) => { await action(ct); });
    }

    /// <summary>
    ///     Executes the provided actions, possibly in parallel.
    /// </summary>
    public static async Task ExecuteInParallelAsync(this ICollection<Func<Task>> actions, TimeSpan timeout)
    {
        using var cts = timeout.ToCancellationTokenSource();
        await ExecuteInParallelAsync(actions, cts.Token);
    }

    /// <summary>
    ///     Executes the provided actions, possibly in parallel.
    /// </summary>
    public static Task ExecuteInParallelAsync(this ICollection<Func<Task>> actions,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(actions);

        return Parallel.ForEachAsync(actions, new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, async (action, _) => { await action(); });
    }
}