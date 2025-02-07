using System.Collections;
using System.Collections.ObjectModel;
using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Collections Extensions
/// </summary>
public static class CollectionsExtensions
{
    /// <summary>
    ///     Applies an action to each item of the given collection
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T>? collection, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (collection is null)
        {
            return;
        }

        foreach (var item in collection)
        {
            action(item);
        }
    }

    /// <summary>
    ///     Applies an action to each item of the given collection
    /// </summary>
    public static async Task ForEachAsync<T>(this IEnumerable<T>? collection, Func<T, Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (collection is null)
        {
            return;
        }

        foreach (var item in collection)
        {
            await action(item);
        }
    }

    /// <summary>
    ///     Adds the `AddRange` to an `IList`
    /// </summary>
    public static bool AddRange<T>([NotNullWhen(returnValue: true)] this ICollection<T>? source,
        [NotNullWhen(returnValue: true)] IEnumerable<T>? newList)
    {
        if (source is null || newList is null)
        {
            return false;
        }

        if (source is List<T> concreteList)
        {
            concreteList.AddRange(newList);

            return true;
        }

        foreach (var element in newList)
        {
            source.Add(element);
        }

        return true;
    }

    /// <summary>
    ///     Return true if the number of elements contained in the source is > 0
    /// </summary>
    public static bool IsNullOrEmpty<T>([NotNullWhen(returnValue: false)] this ICollection<T>? source)
        => source == null || source.Count == 0;

    /// <summary>
    ///     Return true if the number of elements contained in the source is > 0
    /// </summary>
    public static bool IsNullOrEmpty<T>([NotNullWhen(returnValue: false)] this IEnumerable<T>? source)
        => source == null || !source.Any();

    /// <summary>
    ///     Return true if the `source.Take(destination.Count)` elements are equal to destination elements
    /// </summary>
    public static bool StartsWith<T>(this ICollection<T>? source,
        ICollection<T>? destination,
        IEqualityComparer<T>? comparisonType = null)
    {
        if (source == null || destination == null)
        {
            return source == null && destination == null;
        }

        if (IsTargetEmpty(destination) || HasNonEmptyTargetWithEmptySource(source, destination) ||
            IsTargetLongerThanSource(source, destination))
        {
            return false;
        }

        comparisonType ??= EqualityComparer<T>.Default;

        return source.Take(destination.Count).SequenceEqual(destination, comparisonType);
    }

    /// <summary>
    ///     Return true if the `source` elements contain sub-list items
    /// </summary>
    public static bool ContainsNonSequentially<T>(this ICollection<T>? source,
        ICollection<T>? destination,
        IEqualityComparer<T>? comparisonType = null)
    {
        if (source == null || destination == null)
        {
            return source == null && destination == null;
        }

        if (IsTargetEmpty(destination) || HasNonEmptyTargetWithEmptySource(source, destination) ||
            IsTargetLongerThanSource(source, destination))
        {
            return false;
        }

        comparisonType ??= EqualityComparer<T>.Default;

        return !destination.Except(source, comparisonType).Any();
    }

    /// <summary>
    ///     Return true if the `source` elements contain a sub-list sequentially
    /// </summary>
    public static bool ContainsSequentially<T>(this ICollection<T>? source,
        ICollection<T>? destination,
        IEqualityComparer<T>? comparisonType = null)
    {
        if (source == null || destination == null)
        {
            return source == null && destination == null;
        }

        if (IsTargetEmpty(destination) || HasNonEmptyTargetWithEmptySource(source, destination) ||
            IsTargetLongerThanSource(source, destination))
        {
            return false;
        }

        comparisonType ??= EqualityComparer<T>.Default;

        for (var i = 0; i <= source.Count - destination.Count; i++)
        {
            if (source.Skip(i).Take(destination.Count).SequenceEqual(destination, comparisonType))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsTargetEmpty<T>(ICollection<T> destination) => destination.Count == 0;

    private static bool HasNonEmptyTargetWithEmptySource<T>(ICollection<T> source, ICollection<T> destination)
        => source.Count == 0 && destination.Count > 0;

    private static bool IsTargetLongerThanSource<T>(ICollection<T> source, ICollection<T> destination)
        => destination.Count > source.Count;

    /// <summary>
    ///     Return true if the `source.Skip(Math.Max(val1: 0, source.Count - destination.Count))` elements are equal to
    ///     destination elements
    /// </summary>
    public static bool EndsWith<T>(this ICollection<T>? source,
        ICollection<T>? destination,
        IEqualityComparer<T>? comparisonType = null)
    {
        if (source == null || destination == null)
        {
            return source == null && destination == null;
        }

        if (IsTargetEmpty(destination) || HasNonEmptyTargetWithEmptySource(source, destination) ||
            IsTargetLongerThanSource(source, destination))
        {
            return false;
        }

        comparisonType ??= EqualityComparer<T>.Default;

        return source.Skip(Math.Max(val1: 0, source.Count - destination.Count))
            .SequenceEqual(destination, comparisonType);
    }

    /// <summary>
    ///     Return true if the source elements are equal to destination elements
    /// </summary>
    public static bool IsEqualTo<T>(this ICollection<T>? source,
        ICollection<T>? destination,
        IEqualityComparer<T>? comparisonType = null)
    {
        if (source == null || destination == null)
        {
            return source == null && destination == null;
        }

        if (source.Count != destination.Count)
        {
            return false;
        }

        comparisonType ??= EqualityComparer<T>.Default;

        return source.SequenceEqual(destination, comparisonType);
    }

    /// <summary>
    ///     Determine whether any items in values, appear in the input.
    /// </summary>
    public static bool IsIn<T>(this T input, ICollection<T> values, IEqualityComparer<T>? comparisonType = null)
    {
        comparisonType ??= EqualityComparer<T>.Default;

        return values.Any(value => comparisonType.Equals(value, input));
    }

    /// <summary>
    ///     Adds non-null items to the list
    /// </summary>
    public static void AddIfNotNull<T>(this ICollection<T> list, IEnumerable<T>? items)
    {
        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            AddIfNotNull(list, item);
        }
    }

    /// <summary>
    ///     Adds a non-null item to the list
    /// </summary>
    public static void AddIfNotNull<T>(this ICollection<T>? list, T? item)
    {
        if (list == null)
        {
            return;
        }

        if (item is null)
        {
            return;
        }

        list.Add(item);
    }

    /// <summary>
    ///     Gets a value from the dictionary with the given key. Returns default value if it can not find.
    /// </summary>
    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue>? dictionary,
        TKey key,
        TValue? defaultValue = default)
        => dictionary is not null && dictionary.TryGetValue(key, out var obj) ? obj : defaultValue;

    /// <summary>
    ///     Initializes a new instance of the ReadOnlyDictionary class that is a wrapper around the specified dictionary.
    /// </summary>
    public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
        => new ReadOnlyDictionary<TKey, TValue>(dictionary);

    /// <summary>
    ///     Converts the collection of bytes into a hex strings (no prefix).
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static string? ToHex([NotNullIfNotNull(nameof(buffer))] this byte[]? buffer)
    {
        if (buffer is null)
        {
            return null;
        }

        var sb = new StringBuilder(buffer.Length * 2);

        foreach (var @byte in buffer)
        {
            sb.Append(@byte.ToString(format: "X2", CultureInfo.InvariantCulture));
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Counts the number of elements.
    /// </summary>
    public static int Count(this IEnumerable? values)
    {
        if (values is null)
        {
            return 0;
        }

        var count = 0;
        var enumerator = values.GetEnumerator();
        using var enumerator1 = enumerator as IDisposable;

        while (enumerator.MoveNext())
        {
            count++;
        }

        return count;
    }

#if !NET_6
    /// <summary>
    ///     Parses a string into a value.
    /// </summary>
    /// <param name="text"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ParseInvariant<T>(this string text)
        where T : IParsable<T>
        => T.Parse(text, CultureInfo.InvariantCulture);

    /// <summary>
    ///     Tries to parse a string into a value.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryParseInvariant<T>(this string text, out T? result)
        where T : IParsable<T>
        => T.TryParse(text, CultureInfo.InvariantCulture, out result);

    /// <summary>
    ///     Tries to parse and convert a collection of strings into a collection T values.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="ignoreParsingFailures">If it's set to true, adding defaultValue on parsing failures will be ignored. </param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IList<T?> TryConvertToListOfT<T>(this ICollection<string>? items,
        bool ignoreParsingFailures = false,
        T? defaultValue = default)
        where T : IParsable<T>
    {
        var results = new List<T?>();

        if (items is null)
        {
            return results;
        }

        foreach (var item in items)
        {
            if (item.TryParseInvariant<T>(out var result))
            {
                results.Add(result);
            }
            else
            {
                if (!ignoreParsingFailures)
                {
                    results.Add(defaultValue);
                }
            }
        }

        return results;
    }
#endif
}