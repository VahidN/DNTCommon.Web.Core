namespace DNTCommon.Web.Core;

/// <summary>
///     Sync Utils
/// </summary>
public static class SyncUtils
{
    /// <summary>
    ///     Runs a task synchronously.
    ///     Note: Synchronously waiting on tasks or awaiters may cause deadlocks. Use await or JoinableTaskFactory.Run instead.
    /// </summary>
    public static T? Sync<T>(this Task<T>? task) => task is null ? default : task.GetAwaiter().GetResult();

    /// <summary>
    ///     Runs a task synchronously.
    ///     Note: Synchronously waiting on tasks or awaiters may cause deadlocks. Use await or JoinableTaskFactory.Run instead.
    /// </summary>
    public static void Sync(this Task? task) => task?.GetAwaiter().GetResult();

    /// <summary>
    ///     Runs a task synchronously.
    ///     Note: Synchronously waiting on tasks or awaiters may cause deadlocks. Use await or JoinableTaskFactory.Run instead.
    /// </summary>
    public static T? Sync<T>(this ValueTask<T>? task)
        => task is null ? default : task.Value.Preserve().GetAwaiter().GetResult();

    /// <summary>
    ///     Runs a task synchronously.
    ///     Note: Synchronously waiting on tasks or awaiters may cause deadlocks. Use await or JoinableTaskFactory.Run instead.
    /// </summary>
    public static void Sync(this ValueTask? task) => task?.Preserve().GetAwaiter().GetResult();
}
