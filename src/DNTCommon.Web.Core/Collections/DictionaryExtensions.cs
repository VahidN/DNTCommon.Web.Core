using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DNTCommon.Web.Core;

/// <summary>
///     Dictionary Extensions
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    ///     Gets a reference to a TValue in the specified dictionary, adding a new entry with a default value if the key does
    ///     not exist.
    /// </summary>
    public static TValue? GetOrAddDefaultValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        where TKey : notnull
        where TValue : new()
    {
        ArgumentNullException.ThrowIfNull(dict);
        ref var currentValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);

        if (!exists)
        {
            currentValue = new TValue();
        }

        return currentValue;
    }

    /// <summary>
    ///     Gets a reference to a TValue in the specified dictionary, adding a new entry with a default value if the key does
    ///     not exist.
    /// </summary>
    public static TValue? GetOrAddDefaultValue<TKey, TValue>(this Dictionary<TKey, TValue> dict,
        TKey key,
        TValue defaultValue)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dict);
        ref var currentValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);

        if (!exists)
        {
            currentValue = defaultValue;
        }

        return currentValue;
    }

    /// <summary>
    ///     Gets a reference to a TValue in the specified dictionary, adding a new entry from a newValueFunc value if the key
    ///     does not exist.
    /// </summary>
    public static TValue? GetOrAddValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue newValue)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dict);
        ref var currentValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);

        if (!exists)
        {
            currentValue = newValue;
        }

        return currentValue;
    }

    /// <summary>
    ///     Gets the value associated with the specified key.
    /// </summary>
    public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue>? dict, TKey key)
        where TKey : notnull
        => dict?.TryGetValue(key, out var currentValue) == true ? currentValue : default;

    /// <summary>
    ///     Gets the value associated with the specified key.
    /// </summary>
    public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue>? dict,
        TKey key,
        TValue? defaultValue)
        where TKey : notnull
        => dict?.TryGetValue(key, out var currentValue) == true ? currentValue : defaultValue;

    /// <summary>
    ///     It updates a dictionary value only if the key already exists.
    /// </summary>
    public static bool TryUpdateValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue newValue)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ref var valueRef = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);

        if (Unsafe.IsNullRef(ref valueRef))
        {
            return false;
        }

        valueRef = newValue;

        return true;
    }
}
