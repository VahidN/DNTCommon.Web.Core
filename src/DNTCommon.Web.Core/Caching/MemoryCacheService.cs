using Microsoft.Extensions.Caching.Memory;

namespace DNTCommon.Web.Core;

/// <summary>
///     Encapsulates IMemoryCache functionality.
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    /// <summary>
    ///     Encapsulates IMemoryCache functionality.
    /// </summary>
    public MemoryCacheService(IMemoryCache memoryCache)
        => _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

    /// <summary>
    ///     Gets the key's value from the cache.
    /// </summary>
    public T? GetValue<T>(string cacheKey)
        => _memoryCache.Get<T>(cacheKey);

    /// <summary>
    ///     Tries to get the key's value from the cache.
    /// </summary>
    public bool TryGetValue<T>(string cacheKey, out T? result)
        => _memoryCache.TryGetValue(cacheKey, out result);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1)
    {
        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        _memoryCache.Set(cacheKey, factory(), new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        });
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey, Func<T> factory, TimeSpan absoluteExpirationRelativeToNow, int size = 1)
    {
        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        _memoryCache.Set(cacheKey, factory(), new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        });
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration, int size = 1)
        => _memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, T value, TimeSpan absoluteExpirationRelativeToNow, int size = 1)
        => _memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, T value, int size = 1)
        => _memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions
        {
            Size = size // the size limit is the count of entries
        });

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
    ///     Removes the object associated with the given key.
    /// </summary>
    public void Remove(string cacheKey)
        => _memoryCache.Remove(cacheKey);

    private T? GetOrAdd<T>(string cacheKey, Func<T> factory, MemoryCacheEntryOptions options)
    {
        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        // locks get and set internally
        if (_memoryCache.TryGetValue<T>(cacheKey, out var result))
        {
            return result;
        }

        lock (TypeLock<T>.Lock)
        {
            if (_memoryCache.TryGetValue(cacheKey, out result))
            {
                return result;
            }

            result = factory();

            _memoryCache.Set(cacheKey, result, options);

            return result;
        }
    }

    private static class TypeLock<T>
    {
        public static object Lock { get; } = new();
    }
}