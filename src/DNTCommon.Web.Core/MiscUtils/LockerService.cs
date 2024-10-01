using System.Runtime.CompilerServices;
using AsyncKeyedLock;

namespace DNTCommon.Web.Core;

/// <summary>
///     Reader writer locking service
/// </summary>
public class LockerService : ILockerService
{
    private readonly AsyncKeyedLocker<Type> _lock = new(new AsyncKeyedLockOptions());
    private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(value: 5);
    private bool _isDisposed;

    /// <summary>
    ///     Tries to enter the async lock
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<AsyncKeyedLockTimeoutReleaser<Type>> LockAsync<T>(TimeSpan timeout,
        CancellationToken cancellationToken = default)
        where T : notnull
        => _lock.LockAsync(typeof(T), timeout, cancellationToken);

    /// <summary>
    ///     Tries to enter the async lock
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<AsyncKeyedLockTimeoutReleaser<Type>> LockAsync<T>(CancellationToken cancellationToken = default)
        where T : notnull
        => _lock.LockAsync(typeof(T), _lockTimeout, cancellationToken);

    /// <summary>
    ///     Dispose all of the locks
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Tries to enter the sync lock
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Lock<T>(TimeSpan timeout, CancellationToken cancellationToken = default)
        where T : notnull
        => _lock.Lock(typeof(T), timeout, cancellationToken, out _);

    /// <summary>
    ///     Tries to enter the sync lock
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Lock<T>(CancellationToken cancellationToken = default)
        where T : notnull
        => _lock.Lock(typeof(T), _lockTimeout, cancellationToken, out _);

    /// <summary>
    ///     Dispose all of the locks
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        try
        {
            if (disposing)
            {
                _lock.Dispose();
            }
        }
        finally
        {
            _isDisposed = true;
        }
    }
}