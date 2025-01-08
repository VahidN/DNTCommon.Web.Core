using Microsoft.Extensions.Caching.Memory;

namespace DNTCommon.Web.Core;

/// <summary>
///     ICacheService encapsulates IMemoryCache functionality.
/// </summary>
public interface ICacheService
{
    /// <summary>
    ///     Gets all the defined tags. Each tag allows multiple cache entries to be considered as a group.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<string> GetTags();

    /// <summary>
    ///     Removes the object associated with the given key.
    /// </summary>
    void Remove(string cacheKey);

    /// <summary>
    ///     Removes all the cached entries added by this library.
    /// </summary>
    void RemoveAllCachedEntries();

    /// <summary>
    ///     Removes all the tagged cached entries added by this library.
    /// </summary>
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    void RemoveAllCachedEntries(string tag);

    /// <summary>
    ///     Removes all the tagged cached entries added by this library.
    /// </summary>
    void RemoveAllCachedEntries(params ICollection<string> tags);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache,
    ///     otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling await factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    /// <param name="factory"></param>
    /// <param name="absoluteExpiration"></param>
    /// <param name="size"></param>
    Task<T?> GetOrAddAsync<T>(string cacheKey,
        string tag,
        Func<Task<T>> factory,
        DateTimeOffset absoluteExpiration,
        int size = 1);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache,
    ///     otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling await factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    /// <param name="factory"></param>
    /// <param name="absoluteExpirationRelativeToNow"></param>
    /// <param name="size"></param>
    Task<T?> GetOrAddAsync<T>(string cacheKey,
        string tag,
        Func<Task<T>> factory,
        TimeSpan absoluteExpirationRelativeToNow,
        int size = 1);

    /// <summary>
    ///     A thread-safe way (`asynchronously` blocks) of working with memory cache. First tries to get the key's value from
    ///     the cache, otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling await factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    /// <param name="factory"></param>
    /// <param name="options"></param>
    Task<T?> GetOrAddAsync<T>(string cacheKey, string tag, Func<Task<T>> factory, MemoryCacheEntryOptions options);

    /// <summary>
    ///     A thread-safe way (`asynchronously` blocks) of working with memory cache. First tries to get the key's value from
    ///     the cache, otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling await factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tags">Allows multiple cache entries to be considered as a group</param>
    /// <param name="factory"></param>
    /// <param name="options"></param>
    Task<T?> GetOrAddAsync<T>(string cacheKey,
        ICollection<string> tags,
        Func<Task<T>> factory,
        MemoryCacheEntryOptions options);

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
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    T? GetOrAdd<T>(string cacheKey, string tag, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1);

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
    /// <param name="tags">Allows multiple cache entries to be considered as a group</param>
    T? GetOrAdd<T>(string cacheKey,
        ICollection<string> tags,
        Func<T> factory,
        DateTimeOffset absoluteExpiration,
        int size = 1);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache,
    ///     otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    /// <param name="factory"></param>
    /// <param name="absoluteExpirationRelativeToNow"></param>
    /// <param name="size"></param>
    T? GetOrAdd<T>(string cacheKey,
        string tag,
        Func<T> factory,
        TimeSpan absoluteExpirationRelativeToNow,
        int size = 1);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache,
    ///     otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    /// <param name="factory"></param>
    /// <param name="options"></param>
    T? GetOrAdd<T>(string cacheKey, string tag, Func<T> factory, MemoryCacheEntryOptions options);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache,
    ///     otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    ///     The factory argument will be called if there is no item in cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tags">Allows multiple cache entries to be considered as a group</param>
    /// <param name="factory"></param>
    /// <param name="options"></param>
    T? GetOrAdd<T>(string cacheKey, ICollection<string> tags, Func<T> factory, MemoryCacheEntryOptions options);

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
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    void Add<T>(string cacheKey, string tag, T value, DateTimeOffset absoluteExpiration, int size = 1);

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
    /// <param name="tags">Allows multiple cache entries to be considered as a group</param>
    void Add<T>(string cacheKey, ICollection<string> tags, T value, DateTimeOffset absoluteExpiration, int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    /// <param name="factory"></param>
    /// <param name="absoluteExpirationRelativeToNow"></param>
    /// <param name="size"></param>
    void Add<T>(string cacheKey, string tag, Func<T> factory, TimeSpan absoluteExpirationRelativeToNow, int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tags">Allows multiple cache entries to be considered as a group</param>
    /// <param name="factory"></param>
    /// <param name="absoluteExpirationRelativeToNow"></param>
    /// <param name="size"></param>
    void Add<T>(string cacheKey,
        ICollection<string> tags,
        Func<T> factory,
        TimeSpan absoluteExpirationRelativeToNow,
        int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    /// <param name="factory"></param>
    /// <param name="memoryCacheEntryOptions"></param>
    void Add<T>(string cacheKey, string tag, Func<T> factory, MemoryCacheEntryOptions memoryCacheEntryOptions);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tags">Allows multiple cache entries to be considered as a group</param>
    /// <param name="factory"></param>
    /// <param name="memoryCacheEntryOptions"></param>
    void Add<T>(string cacheKey,
        ICollection<string> tags,
        Func<T> factory,
        MemoryCacheEntryOptions memoryCacheEntryOptions);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    /// <param name="value"></param>
    /// <param name="absoluteExpirationRelativeToNow"></param>
    /// <param name="size"></param>
    void Add<T>(string cacheKey, string tag, T value, TimeSpan absoluteExpirationRelativeToNow, int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tags">Allows multiple cache entries to be considered as a group</param>
    /// <param name="value"></param>
    /// <param name="absoluteExpirationRelativeToNow"></param>
    /// <param name="size"></param>
    void Add<T>(string cacheKey,
        ICollection<string> tags,
        T value,
        TimeSpan absoluteExpirationRelativeToNow,
        int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    /// <param name="value"></param>
    /// <param name="memoryCacheEntryOptions"></param>
    void Add<T>(string cacheKey, string tag, T value, MemoryCacheEntryOptions memoryCacheEntryOptions);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="tags">Allows multiple cache entries to be considered as a group</param>
    /// <param name="value"></param>
    /// <param name="memoryCacheEntryOptions"></param>
    void Add<T>(string cacheKey, ICollection<string> tags, T value, MemoryCacheEntryOptions memoryCacheEntryOptions);

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
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    void Add<T>(string cacheKey, string tag, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1);

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
    /// <param name="tags">Allows multiple cache entries to be considered as a group</param>
    void Add<T>(string cacheKey,
        ICollection<string> tags,
        Func<T> factory,
        DateTimeOffset absoluteExpiration,
        int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="value"></param>
    /// <param name="size">
    ///     Gets or sets the size of the cache entry value. If you set it to 1, the size limit will be the count
    ///     of entries.
    /// </param>
    /// <param name="tag">Allows multiple cache entries to be considered as a group</param>
    void Add<T>(string cacheKey, string tag, T value, int size = 1);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="value"></param>
    /// <param name="size">
    ///     Gets or sets the size of the cache entry value. If you set it to 1, the size limit will be the count
    ///     of entries.
    /// </param>
    /// <param name="tags">Allows multiple cache entries to be considered as a group</param>
    void Add<T>(string cacheKey, ICollection<string> tags, T value, int size = 1);
}