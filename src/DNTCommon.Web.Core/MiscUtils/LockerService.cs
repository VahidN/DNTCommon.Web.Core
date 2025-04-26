using System.Runtime.CompilerServices;
using AsyncKeyedLock;

namespace DNTCommon.Web.Core;

/// <summary>
///     Reader writer locking service
/// </summary>
public class LockerService : ILockerService
{
    private readonly AsyncKeyedLocker<Type> _lock = new(new AsyncKeyedLockOptions());
    private bool _isDisposed;

    /// <summary>
    ///     Tries to enter the async lock
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<IDisposable?> LockAsync<T>(TimeSpan timeout, CancellationToken cancellationToken = default)
        where T : notnull
        => _lock.LockOrNullAsync(typeof(T), timeout, cancellationToken);

    /// <summary>
    ///     Tries to enter the async lock
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<IDisposable?> LockAsync(Type key, TimeSpan timeout, CancellationToken cancellationToken = default)
        => _lock.LockOrNullAsync(key, timeout, cancellationToken);

    /// <summary>
    ///     Dispose all the locks
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
    public IDisposable? Lock<T>(TimeSpan timeout, CancellationToken cancellationToken = default)
        where T : notnull
        => _lock.LockOrNull(typeof(T), timeout, cancellationToken);

    /// <summary>
    ///     Tries to enter the sync lock
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable? Lock(Type key, TimeSpan timeout, CancellationToken cancellationToken = default)
        => _lock.LockOrNull(key, timeout, cancellationToken);

    /// <summary>
    ///     Dispose all the locks
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
