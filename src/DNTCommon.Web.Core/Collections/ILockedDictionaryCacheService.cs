namespace DNTCommon.Web.Core;

/// <summary>
///     Represents a thread-safe, ConcurrentDictionaryLocked collection of objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ILockedDictionaryCacheService<T>
{
    /// <summary>
    ///     Returns the current count of items.
    /// </summary>
    int Count { get; }

    /// <summary>
    ///     Gets a value that indicates whether the cache is empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    ///     Adds an item to the cache in a thread-safe manner or updates its value in the cache
    /// </summary>
    void AddOrUpdate(string key, T item);

    /// <summary>
    ///     Adds a key/ value pair to the ConcurrentDictionary by using the specified function if the key does not already
    ///     exist. Returns the new value, or the existing value if the key exists.
    /// </summary>
    T GetOrAdd(string key, T item);

    /// <summary>
    ///     Attempts to get the value associated with the specified key from the ConcurrentDictionary
    /// </summary>
    T? GetOrDefault(string key, T? defaultValue = default);

    /// <summary>
    ///     Removes all values from the cache.
    /// </summary>
    void Clear();

    /// <summary>
    ///     Checks if the key exists in the cache.
    /// </summary>
    bool ContainsKey(string key);

    /// <summary>
    ///     Checks if the key exists in the cache.
    /// </summary>
    bool RemoveKey(string key);

    /// <summary>
    ///     Checks if the item exists in the cache.
    /// </summary>
    bool ContainsItem(T item);
}
