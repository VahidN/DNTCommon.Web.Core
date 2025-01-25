namespace DNTCommon.Web.Core;

/// <summary>
///     ExecuteTimeout Extensions
/// </summary>
public static class TimeoutExtensions
{
    private static void ExecuteTimeoutCommon(Task actionTask, TimeSpan maxDelay)
    {
        var delayTask = Task.Delay(maxDelay);
        var finishedTaskIndex = Task.WaitAny(actionTask, delayTask);

        if (finishedTaskIndex != 0)
        {
            throw new TimeoutException(message: "Action did not finish in the desired time slot.");
        }
    }

    /// <summary>
    ///     Runs an action with a given timeout
    /// </summary>
    /// <param name="func"></param>
    /// <param name="maxDelay"></param>
    /// <typeparam name="T"></typeparam>
    public static void RunWithTimeout<T>(this Func<T> func, TimeSpan maxDelay)
    {
        ArgumentNullException.ThrowIfNull(func);

        var executionTask = Task.Run(func);
        ExecuteTimeoutCommon(executionTask, maxDelay);
    }

    /// <summary>
    ///     Runs an action with a given timeout
    /// </summary>
    /// <param name="action"></param>
    /// <param name="maxDelay"></param>
    public static void RunWithTimeout(this Action action, TimeSpan maxDelay)
    {
        ArgumentNullException.ThrowIfNull(action);

        var executionTask = Task.Run(action);
        ExecuteTimeoutCommon(executionTask, maxDelay);
    }

    /// <summary>
    ///     Runs an action with a given timeout
    /// </summary>
    /// <param name="func"></param>
    /// <param name="maxDelay"></param>
    /// <typeparam name="T"></typeparam>
    public static async Task RunWithTimeoutAsync<T>(this Func<Task<T>> func, TimeSpan maxDelay)
    {
        ArgumentNullException.ThrowIfNull(func);

        var executionTask = Task.Run(func);

        var delayTask = Task.Delay(maxDelay);
        var finishedTask = await Task.WhenAny(executionTask, delayTask);

        if (finishedTask == delayTask)
        {
            throw new TimeoutException(message: "Action did not finish in the desired time slot.");
        }
    }
}