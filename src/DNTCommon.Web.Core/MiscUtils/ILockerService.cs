using AsyncKeyedLock;

namespace DNTCommon.Web.Core;

/// <summary>
///     Reader writer locking service
/// </summary>
public interface ILockerService : IDisposable
{
    /// <summary>
    ///     Tries to enter the sync lock
    /// </summary>
    IDisposable Lock<T>(TimeSpan timeout, CancellationToken cancellationToken = default)
        where T : notnull;

    /// <summary>
    ///     Tries to enter the sync lock
    /// </summary>
    IDisposable Lock<T>(CancellationToken cancellationToken = default)
        where T : notnull;

    /// <summary>
    ///     Tries to enter the async lock
    /// </summary>
    ValueTask<AsyncKeyedLockTimeoutReleaser<Type>> LockAsync<T>(TimeSpan timeout,
        CancellationToken cancellationToken = default)
        where T : notnull;

    /// <summary>
    ///     Tries to enter the async lock
    /// </summary>
    ValueTask<AsyncKeyedLockTimeoutReleaser<Type>> LockAsync<T>(CancellationToken cancellationToken = default)
        where T : notnull;
}