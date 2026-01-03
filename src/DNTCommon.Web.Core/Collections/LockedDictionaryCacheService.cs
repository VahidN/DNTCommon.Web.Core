namespace DNTCommon.Web.Core;

/// <summary>
///     Represents a thread-safe, ConcurrentDictionaryLocked collection of objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public class LockedDictionaryCacheService<T> : ILockedDictionaryCacheService<T>
{
    private readonly ConcurrentDictionaryLocked<string, T> _items = new(StringComparer.Ordinal);

    /// <summary>
    ///     Returns the current count of items.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    ///     Gets a value that indicates whether the cache is empty.
    /// </summary>
    public bool IsEmpty => _items.IsEmpty;

    /// <summary>
    ///     Adds an item to the cache in a thread-safe manner or updates its value in the cache
    /// </summary>
    public void AddOrUpdate(string key, T item) => _items.LockedAddOrUpdate(key, item, (_, _) => item);

    /// <summary>
    ///     Adds a key/ value pair to the ConcurrentDictionary by using the specified function if the key does not already
    ///     exist. Returns the new value, or the existing value if the key exists.
    /// </summary>
    public T GetOrAdd(string key, T item) => _items.LockedGetOrAdd(key, _ => item);

    /// <summary>
    ///     Attempts to get the value associated with the specified key from the ConcurrentDictionary
    /// </summary>
    public T? GetOrDefault(string key, T? defaultValue = default)
        => _items.TryGetValue(key, out var item) ? item.Value : defaultValue;

    /// <summary>
    ///     Removes all values from the cache.
    /// </summary>
    public void Clear() => _items.Clear();

    /// <summary>
    ///     Checks if the key exists in the cache.
    /// </summary>
    public bool ContainsKey(string key) => _items.ContainsKey(key);

    /// <summary>
    ///     Checks if the key exists in the cache.
    /// </summary>
    public bool RemoveKey(string key) => _items.Remove(key, out _);

    /// <summary>
    ///     Checks if the item exists in the cache.
    /// </summary>
    public bool ContainsItem(T item)
        => _items.Values.Contains(new Lazy<T>(() => item, LazyThreadSafetyMode.ExecutionAndPublication));
}
