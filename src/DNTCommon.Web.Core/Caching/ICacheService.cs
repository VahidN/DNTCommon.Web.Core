using Microsoft.Extensions.Caching.Memory;

namespace DNTCommon.Web.Core;

/// <summary>
///     ICacheService encapsulates IMemoryCache functionality.
/// </summary>
public interface ICacheService
{
    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache,
    ///     otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling await factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    Task<T?> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory, DateTimeOffset absoluteExpiration, int size = 1);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache,
    ///     otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling await factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    Task<T?> GetOrAddAsync<T>(string cacheKey,
        Func<Task<T>> factory,
        TimeSpan absoluteExpirationRelativeToNow,
        int size = 1);

    /// <summary>
    ///     A thread-safe way (`asynchronously` blocks) of working with memory cache. First tries to get the key's value from
    ///     the cache, otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling await factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    Task<T?> GetOrAddAsync<T>(string cacheKey, Func<Task<T>> factory, MemoryCacheEntryOptions options);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache,
    ///     otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    ///     The factory argument will be called if there is no item in cache.
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
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache,
    ///     otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    T? GetOrAdd<T>(string cacheKey, Func<T> factory, TimeSpan absoluteExpirationRelativeToNow, int size = 1);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache,
    ///     otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    T? GetOrAdd<T>(string cacheKey, Func<T> factory, MemoryCacheEntryOptions options);

    /// <summary>
    ///     Gets the key's value from the cache.
    ///     Return the value associated with this key, or default(TItem) if the key is not present.
    /// </summary>
    T? GetValue<T>(string cacheKey);

    /// <summary>
    ///     Tries to get the key's value from the cache.
    ///     Returns true if the key was found. false otherwise.
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
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    void Add<T>(string cacheKey, Func<T> factory, MemoryCacheEntryOptions memoryCacheEntryOptions);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    void Add<T>(string cacheKey, T value, TimeSpan absoluteExpirationRelativeToNow, int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    void Add<T>(string cacheKey, T value, MemoryCacheEntryOptions memoryCacheEntryOptions);

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