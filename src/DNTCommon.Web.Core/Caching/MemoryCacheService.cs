using Microsoft.Extensions.Caching.Memory;

namespace DNTCommon.Web.Core;

/// <summary>
///     Encapsulates IMemoryCache functionality.
/// </summary>
/// <remarks>
///     Encapsulates IMemoryCache functionality.
/// </remarks>
public class MemoryCacheService(IMemoryCache memoryCache, ILockerService lockerService) : ICacheService
{
    /// <summary>
    ///     Gets the key's value from the cache.
    /// </summary>
    public T? GetValue<T>(string cacheKey) => memoryCache.Get<T>(cacheKey);

    /// <summary>
    ///     Tries to get the key's value from the cache.
    /// </summary>
    public bool TryGetValue<T>(string cacheKey, out T? result) => memoryCache.TryGetValue(cacheKey, out result);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1)
        => Add(cacheKey, factory, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey, Func<T> factory, MemoryCacheEntryOptions memoryCacheEntryOptions)
    {
        ArgumentNullException.ThrowIfNull(factory);

        memoryCache.Set(cacheKey, factory(), memoryCacheEntryOptions);
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey, Func<T> factory, TimeSpan absoluteExpirationRelativeToNow, int size = 1)
        => Add(cacheKey, factory, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration, int size = 1)
        => memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, T value, TimeSpan absoluteExpirationRelativeToNow, int size = 1)
        => memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, T value, int size = 1)
        => memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
        {
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, T value, MemoryCacheEntryOptions memoryCacheEntryOptions)
        => memoryCache.Set(cacheKey, value, memoryCacheEntryOptions);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    /// </summary>
    public T? GetOrAdd<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1)
        => GetOrAdd(cacheKey, factory, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    /// </summary>
    public T? GetOrAdd<T>(string cacheKey, Func<T> factory, TimeSpan absoluteExpirationRelativeToNow, int size = 1)
        => GetOrAdd(cacheKey, factory, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    /// </summary>
    public T? GetOrAdd<T>(string cacheKey, Func<T> factory, MemoryCacheEntryOptions options)
    {
        ArgumentNullException.ThrowIfNull(factory);

        // locks get and set internally

        using var locker = lockerService.Lock<MemoryCacheService>();

        if (memoryCache.TryGetValue<T>(cacheKey, out var result))
        {
            return result;
        }

        result = factory();
        memoryCache.Set(cacheKey, result, options);

        return result;
    }

    /// <summary>
    ///     Removes the object associated with the given key.
    /// </summary>
    public void Remove(string cacheKey) => memoryCache.Remove(cacheKey);
}