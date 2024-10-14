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
}