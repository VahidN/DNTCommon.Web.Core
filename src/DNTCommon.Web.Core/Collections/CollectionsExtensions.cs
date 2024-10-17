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
    ///     Return true if the source elements are equal to destination elements
    /// </summary>
    public static bool IsEqualTo<T>(this IList<T>? source, IList<T>? destination)
    {
        if (source == null || destination == null)
        {
            return source == null && destination == null;
        }

        if (source.Count != destination.Count)
        {
            return false;
        }

        var comparer = EqualityComparer<T>.Default;

        return !source.Where((t, i) => !comparer.Equals(t, destination[i])).Any();
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
}