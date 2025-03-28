using System.Collections.Concurrent;

namespace DNTCommon.Web.Core;

/// <summary>
///     'GetOrAdd' call on the ConcurrentDictionary is not thread safe and we might end up creating the GetterInfo more
///     than once. To prevent this Lazy is used. In the worst case multiple Lazy objects are created for multiple
///     threads but only one of the objects succeeds in creating a GetterInfo.
/// </summary>
/// <remarks>
///     A thread-safe ConcurrentDictionary
/// </remarks>
/// <param name="comparer"></param>
public class ConcurrentDictionaryLocked<TKey, TValue>(IEqualityComparer<TKey> comparer)
    : ConcurrentDictionary<TKey, Lazy<TValue>>(comparer)
    where TKey : notnull
{
    /// <summary>
    ///     Adds a key/ value pair to the ConcurrentDictionary by using the specified function if the key does not already
    ///     exist. Returns the new value, or the existing value if the key exists.
    /// </summary>
    public TValue LockedGetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        => GetOrAdd(key,
                static (k, arg) => new Lazy<TValue>(() => arg(k), LazyThreadSafetyMode.ExecutionAndPublication),
                valueFactory)
            .Value;

    /// <summary>
    ///     Adds a key/ value pair to the ConcurrentDictionary if the key does not already exist, or updates a key/ value pair
    ///     in the ConcurrentDictionary by using the specified function if the key already exists.
    /// </summary>
    public TValue LockedAddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        => AddOrUpdate(key, new Lazy<TValue>(() => addValue),
                (k, currentValue) => new Lazy<TValue>(() => updateValueFactory(k, currentValue.Value),
                    LazyThreadSafetyMode.ExecutionAndPublication))
            .Value;

    /// <summary>
    ///     Uses the specified functions to add a key/ value pair to the ConcurrentDictionary if the key does not already
    ///     exist, or to update a key/ value pair in the ConcurrentDictionary if the key already exists.
    /// </summary>
    public TValue LockedAddOrUpdate(TKey key,
        Func<TKey, TValue> addValueFactory,
        Func<TKey, TValue, TValue> updateValueFactory)
        => AddOrUpdate(key, k => new Lazy<TValue>(() => addValueFactory(k)),
                (k, currentValue) => new Lazy<TValue>(() => updateValueFactory(k, currentValue.Value),
                    LazyThreadSafetyMode.ExecutionAndPublication))
            .Value;
}
