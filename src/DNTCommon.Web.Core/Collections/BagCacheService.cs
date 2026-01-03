using System.Collections.Concurrent;

namespace DNTCommon.Web.Core;

/// <summary>
///     Represents a thread-safe, unordered collection of objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public class BagCacheService<T> : IBagCacheService<T>
{
    // Thread-safe unordered collection
    private readonly ConcurrentBag<T> _bag = [];

    /// <summary>
    ///     Returns the current count of items.
    /// </summary>
    public int Count => _bag.Count;

    /// <summary>
    ///     Gets a value that indicates whether the cache is empty.
    /// </summary>
    public bool IsEmpty => _bag.IsEmpty;

    /// <summary>
    ///     Adds an item to the cache in a thread-safe manner.
    /// </summary>
    public void Add(T item) => _bag.Add(item);

    /// <summary>
    ///     Removes all values from the cache.
    /// </summary>
    public void Clear() => _bag.Clear();

    /// <summary>
    ///     Checks if the item exists in the bag.
    ///     Note: This is an O(n) operation as ConcurrentBag is unordered.
    /// </summary>
    public bool Contains(T item) => _bag.Contains(item);

    /// <summary>
    ///     Copies the elements to a new array.
    /// </summary>
    public T[] ToArray() => [.. _bag];
}
