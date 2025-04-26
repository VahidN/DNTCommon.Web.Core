namespace DNTCommon.Web.Core;

/// <summary>
///     Reader writer locking service
/// </summary>
public interface ILockerService : IDisposable
{
    /// <summary>
    ///     Tries to enter the sync lock
    /// </summary>
    public IDisposable? Lock(Type key, TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Tries to enter the sync lock
    /// </summary>
    public IDisposable? Lock<T>(TimeSpan timeout, CancellationToken cancellationToken = default)
        where T : notnull;

    /// <summary>
    ///     Tries to enter the async lock
    /// </summary>
    public ValueTask<IDisposable?> LockAsync(Type key, TimeSpan timeout, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Tries to enter the async lock
    /// </summary>
    public ValueTask<IDisposable?> LockAsync<T>(TimeSpan timeout, CancellationToken cancellationToken = default)
        where T : notnull;
}
