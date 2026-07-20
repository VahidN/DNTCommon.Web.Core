using System.Runtime.CompilerServices;
using AsyncKeyedLock;

namespace DNTCommon.Web.Core;

/// <summary>
///     Reader writer locking service
/// </summary>
public class LockerService : ILockerService
{
    private readonly AsyncKeyedLocker<string> _keyedLocker = new(StringComparer.Ordinal);
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
                _keyedLocker.Dispose();
            }
        }
        finally
        {
            _isDisposed = true;
        }
    }

    /// <summary>
    ///     Asynchronously locks and executes the action
    /// </summary>
    public async Task<T> ExecuteWithLockAsync<T>(string lockKey,
        TimeSpan timeout,
        Func<CancellationToken, Task<T>> func,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(func);

        if (string.IsNullOrWhiteSpace(lockKey))
        {
            throw new ArgumentNullException(nameof(lockKey));
        }

        using (await _keyedLocker.LockOrNullAsync(lockKey, timeout, cancellationToken))
        {
            return await func(cancellationToken);
        }
    }

    /// <summary>
    ///     Synchronously locks and executes the action
    /// </summary>
    public T ExecuteWithLock<T>(string lockKey,
        TimeSpan timeout,
        Func<T> func,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(func);

        if (string.IsNullOrWhiteSpace(lockKey))
        {
            throw new ArgumentNullException(nameof(lockKey));
        }

        using (_keyedLocker.LockOrNull(lockKey, timeout, cancellationToken))
        {
            return func();
        }
    }
}
