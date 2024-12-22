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
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        };

        options.ExpirationTokens.Add(signal.GetChangeToken(tag));

        Add(cacheKey, tag, factory, options);
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    ///     It will use the factory method to get the value and then inserts it.
    /// </summary>
    public void Add<T>(string cacheKey, string tag, Func<T> factory, MemoryCacheEntryOptions memoryCacheEntryOptions)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(memoryCacheEntryOptions);

        memoryCacheEntryOptions.ExpirationTokens.Add(signal.GetChangeToken(tag));

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
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        };

        options.ExpirationTokens.Add(signal.GetChangeToken(tag));

        Add(cacheKey, tag, factory, options);
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, string tag, T value, DateTimeOffset absoluteExpiration, int size = 1)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        };

        options.ExpirationTokens.Add(signal.GetChangeToken(tag));

        memoryCache.Set(cacheKey, value, options);
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, string tag, T value, TimeSpan absoluteExpirationRelativeToNow, int size = 1)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        };

        options.ExpirationTokens.Add(signal.GetChangeToken(tag));

        memoryCache.Set(cacheKey, value, options);
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, string tag, T value, int size = 1)
    {
        var options = new MemoryCacheEntryOptions
        {
            Size = size // the size limit is the count of entries
        };

        options.ExpirationTokens.Add(signal.GetChangeToken(tag));

        memoryCache.Set(cacheKey, value, options);
    }

    /// <summary>
    ///     Adds a key-value to the cache.
    /// </summary>
    public void Add<T>(string cacheKey, string tag, T value, MemoryCacheEntryOptions memoryCacheEntryOptions)
    {
        ArgumentNullException.ThrowIfNull(memoryCacheEntryOptions);

        memoryCacheEntryOptions.ExpirationTokens.Add(signal.GetChangeToken(tag));
        memoryCache.Set(cacheKey, value, memoryCacheEntryOptions);
    }

    /// <summary>
    ///     A thread-safe way of working with memory cache. First tries to get the key's value from the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    /// </summary>
    public T? GetOrAdd<T>(string cacheKey, string tag, Func<T> factory, DateTimeOffset absoluteExpiration, int size = 1)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        };

        options.ExpirationTokens.Add(signal.GetChangeToken(tag));

        return GetOrAdd(cacheKey, tag, factory, options);
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
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        };

        options.ExpirationTokens.Add(signal.GetChangeToken(tag));

        return GetOrAdd(cacheKey, tag, factory, options);
    }

    /// <summary>
    ///     A thread-safe way (`synchronously` blocks) of working with memory cache. First tries to get the key's value from
    ///     the cache.
    ///     Otherwise it will use the factory method to get the value and then inserts it.
    ///     The returned value is a found item in cache or the result of calling factory().
    /// </summary>
    public T? GetOrAdd<T>(string cacheKey, string tag, Func<T> factory, MemoryCacheEntryOptions options)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(options);

        // locks get and set internally

        using var locker = lockerService.Lock<MemoryCacheService>();

        if (memoryCache.TryGetValue<T>(cacheKey, out var result))
        {
            return result;
        }

        result = factory();

        options.ExpirationTokens.Add(signal.GetChangeToken(tag));
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
    public async Task<T?> GetOrAddAsync<T>(string cacheKey,
        string tag,
        Func<Task<T>> factory,
        MemoryCacheEntryOptions options)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(options);

        // locks get and set internally

        using var locker = await lockerService.LockAsync<MemoryCacheService>();

        if (memoryCache.TryGetValue<T>(cacheKey, out var result))
        {
            return result;
        }

        result = await factory();

        options.ExpirationTokens.Add(signal.GetChangeToken(tag));
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
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            Size = size // the size limit is the count of entries
        };

        options.ExpirationTokens.Add(signal.GetChangeToken(tag));

        return GetOrAddAsync(cacheKey, tag, factory, options);
    }

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
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            Size = size // the size limit is the count of entries
        };

        options.ExpirationTokens.Add(signal.GetChangeToken(tag));

        return GetOrAddAsync(cacheKey, tag, factory, options);
    }
}