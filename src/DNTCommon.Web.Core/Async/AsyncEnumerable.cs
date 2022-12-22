namespace DNTCommon.Web.Core;

/// <summary>
///     IAsyncEnumerable Extensions
/// </summary>
public static class AsyncEnumerable
{
    /// <summary>
    ///     Creates an empty IAsyncEnumerator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IAsyncEnumerator<T> EmptyEnumerator<T>() => new EmptyAsyncEnumerator<T>();

    /// <summary>
    ///     Creates an empty IAsyncEnumerable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IAsyncEnumerable<T> EmptyAsync<T>() => new EmptyAsyncEnumerator<T>();

    private sealed class EmptyAsyncEnumerator<T> : IAsyncEnumerator<T>, IAsyncEnumerable<T>
    {
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return this;
        }

        public T Current => default!;
        public ValueTask DisposeAsync() => default;

        public ValueTask<bool> MoveNextAsync() => new(false);
    }
}