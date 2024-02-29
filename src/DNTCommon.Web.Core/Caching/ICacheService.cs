namespace DNTCommon.Web.Core;

/// <summary>
///     ICacheService encapsulates IMemoryCache functionality.
/// </summary>
public interface ICacheService
{
    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="factory"></param>
    /// <param name="absoluteExpiration"></param>
    /// <param name="size">
    ///     Gets or sets the size of the cache entry value. If you set it to 1, the size limit will be the count
    ///     of entries.
    /// </param>
    /// <typeparam name="T"></typeparam>
    T? GetOrAdd<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    /// </summary>
    T? GetOrAdd<T>(string cacheKey, Func<T> factory, TimeSpan absoluteExpirationRelativeToNow, int size = 1);

    /// <summary>
    ///     Gets the key's value from the cache.
    /// </summary>
    T? GetValue<T>(string cacheKey);

    /// <summary>
    ///     Tries to get the key's value from the cache.
    /// </summary>
    bool TryGetValue<T>(string cacheKey, out T? result);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="value"></param>
    /// <param name="absoluteExpiration"></param>
    /// <param name="size">
    ///     Gets or sets the size of the cache entry value. If you set it to 1, the size limit will be the count
    ///     of entries.
    /// </param>
    /// <typeparam name="T"></typeparam>
    void Add<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration, int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    void Add<T>(string cacheKey, Func<T> factory, TimeSpan absoluteExpirationRelativeToNow, int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    void Add<T>(string cacheKey, T value, TimeSpan absoluteExpirationRelativeToNow, int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="factory"></param>
    /// <param name="absoluteExpiration"></param>
    /// <param name="size">
    ///     Gets or sets the size of the cache entry value. If you set it to 1, the size limit will be the count
    ///     of entries.
    /// </param>
    /// <typeparam name="T"></typeparam>
    void Add<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="value"></param>
    /// <param name="size">
    ///     Gets or sets the size of the cache entry value. If you set it to 1, the size limit will be the count
    ///     of entries.
    /// </param>
    /// <typeparam name="T"></typeparam>
    void Add<T>(string cacheKey, T value, int size = 1);

    /// <summary>
    ///     Removes the object associated with the given key.
    /// </summary>
    void Remove(string cacheKey);
}