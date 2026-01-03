namespace DNTCommon.Web.Core;

/// <summary>
///     Represents a thread-safe, unordered collection of objects.
/// </summary>
public interface IBagCacheService<T>
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
    ///     Adds an item to the cache in a thread-safe manner.
    /// </summary>
    void Add(T item);

    /// <summary>
    ///     Removes all values from the cache.
    /// </summary>
    void Clear();

    /// <summary>
    ///     Checks if the item exists in the bag.
    ///     Note: This is an O(n) operation as ConcurrentBag is unordered.
    /// </summary>
    bool Contains(T item);

    /// <summary>
    ///     Copies the elements to a new array.
    /// </summary>
    T[] ToArray();
}
