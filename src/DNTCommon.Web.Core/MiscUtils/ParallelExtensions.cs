namespace DNTCommon.Web.Core;

/// <summary>
///     Parallel Extensions
/// </summary>
public static class ParallelExtensions
{
    /// <summary>
    ///     Executes the provided action multiple times, possibly in parallel.
    /// </summary>
    public static void ExecuteInParallel(Action test, int times, CancellationToken cancellationToken = default)
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
    public static void ExecuteInParallel(ICollection<Action> actions, CancellationToken cancellationToken = default)
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
    public static Task ExecuteInParallelAsync<TParameter>(ICollection<TParameter> parameters,
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
    public static Task ExecuteInParallelAsync<TParameter>(ICollection<TParameter> parameters,
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
    public static Task ExecuteInParallelAsync(ICollection<Func<CancellationToken, Task>> actions,
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
    public static Task ExecuteInParallelAsync(ICollection<Func<Task>> actions,
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