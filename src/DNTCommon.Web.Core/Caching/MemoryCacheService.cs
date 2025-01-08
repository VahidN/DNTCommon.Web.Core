using Microsoft.Extensions.Caching.Memory;

namespace DNTCommon.Web.Core;

/// <summary>
///     Encapsulates IMemoryCache functionality.
/// </summary>
/// <remarks>
///     Encapsulates IMemoryCache functionality.
/// </remarks>
public class MemoryCacheService(
    IMemoryCache memoryCache,
    IMemoryCacheResetTokenProvider signal,
    ILockerService lockerService) : ICacheService
{
    private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(value: 5);

    /// <summary>
    ///     Gets all the defined tags. Each tag allows multiple cache entries to be considered as a group.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<string> GetTags() => signal.GetTags();

    /// <summary>
    ///     Removes  all the cached entries added by this library.
    /// </summary>
    public void RemoveAllCachedEntries() => signal.RemoveAllChangeTokens();

    /// <summary>
    ///     Removes all the tagged cached entries added by this library.
    /// </summary>
    public void RemoveAllCachedEntries(string tag) => signal.RemoveChangeToken(tag);

    /// <summary>
    ///     Removes all the tagged cached entries added by this library.
    /// </summary>
    public void RemoveAllCachedEntries(params ICollection<string> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        foreach (var tag in tags)
        {
            signal.RemoveChangeToken(tag);
        }
    }

    /// <summary>
    ///     Gets the key's value from the cache.
    ///     Return the value associated with this key, or default(TItem) if the key is not present.
    /// </summary>
    public T? GetValue<T>(string cacheKey) => memoryCache.Get<T>(cacheKey);

    /// <summary>
    ///     Tries to get the key's value from the cache.
    ///     Returns true if the key was found. false otherwise.
    /// </summary>
    public bool TryGetValue<T>(string cacheKey, out T? result) => memoryCache.TryGetValue(cacheKey, out result);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey, string tag, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1)
        => Add(cacheKey, [tag], factory, absoluteExpiration, size);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey,
        ICollection<string> tags,
        Func<T> factory,
        DateTimeOffset absoluteExpiration,
        int size = 1)
        => Add(cacheKey, tags, factory, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey, string tag, Func<T> factory, MemoryCacheEntryOptions memoryCacheEntryOptions)
        => Add(cacheKey, [tag], factory, memoryCacheEntryOptions);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey,
        ICollection<string> tags,
        Func<T> factory,
        MemoryCacheEntryOptions memoryCacheEntryOptions)
    {
        ArgumentNullException.ThrowIfNull(tags);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(memoryCacheEntryOptions);

        foreach (var tag in tags)
        {
            memoryCacheEntryOptions.ExpirationTokens.Add(signal.GetChangeToken(tag));
        }

        memoryCache.Set(cacheKey, factory(), memoryCacheEntryOptions);
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey,
        string tag,
        Func<T> factory,
        TimeSpan absoluteExpirationRelativeToNow,
        int size = 1)
        => Add(cacheKey, [tag], factory, absoluteExpirationRelativeToNow, size);

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey,
        ICollection<string> tags,
        Func<T> factory,
        TimeSpan absoluteExpirationRelativeToNow,
        int size = 1)
        => Add(cacheKey, tags, factory, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, string tag, T value, DateTimeOffset absoluteExpiration, int size = 1)
        => Add(cacheKey, [tag], value, absoluteExpiration, size);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey,
        ICollection<string> tags,
        T value,
        DateTimeOffset absoluteExpiration,
        int size = 1)
    {
        ArgumentNullException.ThrowIfNull(tags);

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        };

        foreach (var tag in tags)
        {
            options.ExpirationTokens.Add(signal.GetChangeToken(tag));
        }

        memoryCache.Set(cacheKey, value, options);
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, string tag, T value, TimeSpan absoluteExpirationRelativeToNow, int size = 1)
        => Add(cacheKey, [tag], value, absoluteExpirationRelativeToNow, size);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey,
        ICollection<string> tags,
        T value,
        TimeSpan absoluteExpirationRelativeToNow,
        int size = 1)
    {
        ArgumentNullException.ThrowIfNull(tags);

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        };

        foreach (var tag in tags)
        {
            options.ExpirationTokens.Add(signal.GetChangeToken(tag));
        }

        memoryCache.Set(cacheKey, value, options);
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, string tag, T value, int size = 1) => Add(cacheKey, [tag], value, size);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, ICollection<string> tags, T value, int size = 1)
    {
        ArgumentNullException.ThrowIfNull(tags);

        var options = new MemoryCacheEntryOptions
        {
            Size = size // the size limit is the count of entries
        };

        foreach (var tag in tags)
        {
            options.ExpirationTokens.Add(signal.GetChangeToken(tag));
        }

        memoryCache.Set(cacheKey, value, options);
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, string tag, T value, MemoryCacheEntryOptions memoryCacheEntryOptions)
        => Add(cacheKey, [tag], value, memoryCacheEntryOptions);

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey,
        ICollection<string> tags,
        T value,
        MemoryCacheEntryOptions memoryCacheEntryOptions)
    {
        ArgumentNullException.ThrowIfNull(tags);
        ArgumentNullException.ThrowIfNull(memoryCacheEntryOptions);

        foreach (var tag in tags)
        {
            memoryCacheEntryOptions.ExpirationTokens.Add(signal.GetChangeToken(tag));
        }

        memoryCache.Set(cacheKey, value, memoryCacheEntryOptions);
    }

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    /// </summary>
    public T? GetOrAdd<T>(string cacheKey,
        string tag,
        Func<T> factory,
        TimeSpan absoluteExpirationRelativeToNow,
        int size = 1)
        => GetOrAdd(cacheKey, [tag], factory, absoluteExpirationRelativeToNow, size);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    /// </summary>
    public T? GetOrAdd<T>(string cacheKey, string tag, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1)
        => GetOrAdd(cacheKey, [tag], factory, absoluteExpiration, size);

    /// <summary>
    ///     A thread-safe way (`synchronously` blocks) of working with memory cache. First tries to get the key's value from
    ///     the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    /// </summary>
    public T? GetOrAdd<T>(string cacheKey, string tag, Func<T> factory, MemoryCacheEntryOptions options)
        => GetOrAdd(cacheKey, [tag], factory, options);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    /// </summary>
    public T? GetOrAdd<T>(string cacheKey,
        ICollection<string> tags,
        Func<T> factory,
        DateTimeOffset absoluteExpiration,
        int size = 1)
        => GetOrAdd(cacheKey, tags, factory, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     A thread-safe way (`synchronously` blocks) of working with memory cache. First tries to get the key's value from
    ///     the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    /// </summary>
    public T? GetOrAdd<T>(string cacheKey, ICollection<string> tags, Func<T> factory, MemoryCacheEntryOptions options)
    {
        ArgumentNullException.ThrowIfNull(tags);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(options);

        // locks get and set internally

        using var locker = lockerService.Lock<MemoryCacheService>(_lockTimeout);

        if (memoryCache.TryGetValue<T>(cacheKey, out var result))
        {
            return result;
        }

        result = factory();

        foreach (var tag in tags)
        {
            options.ExpirationTokens.Add(signal.GetChangeToken(tag));
        }

        memoryCache.Set(cacheKey, result, options);

        return result;
    }

    /// <summary>
    ///     Removes the object associated with the given key.
    /// </summary>
    public void Remove(string cacheKey) => memoryCache.Remove(cacheKey);

    /// <summary>
    ///     A thread-safe way (`asynchronously` blocks) of working with memory cache. First tries to get the key's value from
    ///     the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned values is a found item in cache or the result of calling await factory().
    /// </summary>
    public Task<T?> GetOrAddAsync<T>(string cacheKey,
        string tag,
        Func<Task<T>> factory,
        MemoryCacheEntryOptions options)
        => GetOrAddAsync(cacheKey, [tag], factory, options);

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned values is a found item in cache or the result of calling await factory().
    /// </summary>
    public async Task<T?> GetOrAddAsync<T>(string cacheKey,
        ICollection<string> tags,
        Func<Task<T>> factory,
        MemoryCacheEntryOptions options)
    {
        ArgumentNullException.ThrowIfNull(tags);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(options);

        // locks get and set internally

        using var locker = await lockerService.LockAsync<MemoryCacheService>(_lockTimeout);

        if (memoryCache.TryGetValue<T>(cacheKey, out var result))
        {
            return result;
        }

        result = await factory();

        foreach (var tag in tags)
        {
            options.ExpirationTokens.Add(signal.GetChangeToken(tag));
        }

        memoryCache.Set(cacheKey, result, options);

        return result;
    }

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned values is a found item in cache or the result of calling await factory().
    /// </summary>
    public Task<T?> GetOrAddAsync<T>(string cacheKey,
        string tag,
        Func<Task<T>> factory,
        DateTimeOffset absoluteExpiration,
        int size = 1)
        => GetOrAddAsync(cacheKey, [tag], factory, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned values is a found item in cache or the result of calling await factory().
    /// </summary>
    public Task<T?> GetOrAddAsync<T>(string cacheKey,
        string tag,
        Func<Task<T>> factory,
        TimeSpan absoluteExpirationRelativeToNow,
        int size = 1)
        => GetOrAddAsync(cacheKey, [tag], factory, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        });

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    /// </summary>
    public T? GetOrAdd<T>(string cacheKey,
        ICollection<string> tags,
        Func<T> factory,
        TimeSpan absoluteExpirationRelativeToNow,
        int size = 1)
        => GetOrAdd(cacheKey, tags, factory, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        });
}