using System.Numerics;

namespace DNTCommon.Web.Core;

/// <summary>
///     Converting string values to numbers
/// </summary>
public static class NumbersExtensions
{
    /// <summary>
    ///     Creates an array of bytes with a cryptographically strong random sequence of values.
    /// </summary>
    /// <param name="count">The number of bytes of random values to create.</param>
    /// <returns>An array populated with cryptographically strong random values.</returns>
    public static byte[] RandomBytes(this int count) => RandomNumberGenerator.GetBytes(count);

    /// <summary>
    ///     Fills a span with cryptographically strong random bytes.
    /// </summary>
    /// <param name="randomBytes">The span to fill with cryptographically strong random bytes.</param>
    public static void FillWithRandomBytes(this byte[] randomBytes) => RandomNumberGenerator.Fill(randomBytes);

    /// <summary>
    ///     Returns a random item from the given array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="values">The array to get a random item from.</param>
    /// <returns>A random item from the array, or default(T) if the array is null or empty.</returns>
    public static T? GetRandomItem<T>([NotNullIfNotNull(nameof(values))] this T[]? values)
        => values is null || values.Length == 0 ? default : values[RandomNumberGenerator.GetInt32(values.Length)];

    /// <summary>
    ///     Returns a random item from the given list.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    /// <param name="values">The list to get a random item from.</param>
    /// <returns>A random item from the list, or default(T) if the list is null or empty.</returns>
    public static T? GetRandomItem<T>([NotNullIfNotNull(nameof(values))] this List<T>? values)
        => values is null || values.Count == 0 ? default : values[RandomNumberGenerator.GetInt32(values.Count)];

    /// <summary>
    ///     Returns a random item from the given read-only list.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    /// <param name="values">The read-only list to get a random item from.</param>
    /// <returns>A random item from the list, or default(T) if the list is null or empty.</returns>
    public static T? GetRandomItem<T>([NotNullIfNotNull(nameof(values))] this IReadOnlyList<T>? values)
        => values is null || values.Count == 0 ? default : values[RandomNumberGenerator.GetInt32(values.Count)];

    /// <summary>
    ///     Returns a random item from the given read-only span.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the span.</typeparam>
    /// <param name="values">The read-only span to get a random item from.</param>
    /// <returns>A random item from the span, or default(T) if the span is empty.</returns>
    public static T? GetRandomItem<T>(this ReadOnlySpan<T> values)
        => values.IsEmpty ? default : values[RandomNumberGenerator.GetInt32(values.Length)];

    /// <summary>
    ///     Returns a random item from the given span.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the span.</typeparam>
    /// <param name="values">The span to get a random item from.</param>
    /// <returns>A random item from the span, or default(T) if the span is empty.</returns>
    public static T? GetRandomItem<T>(this Span<T> values)
        => values.IsEmpty ? default : values[RandomNumberGenerator.GetInt32(values.Length)];

    /// <summary>
    ///     Returns a random item from the given sequence.
    ///     This method will iterate through the sequence. For collections that have a known count,
    ///     consider using a more specific overload (e.g., for List of T or T[]).
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="values">The sequence to get a random item from.</param>
    /// <returns>A random item from the sequence, or default(T) if the sequence is null or empty.</returns>
    public static T? GetRandomItem<T>([NotNullIfNotNull(nameof(values))] this IEnumerable<T>? values)
    {
        if (values is null)
        {
            return default;
        }

        // Optimization for collections that have an efficient Count and indexer
        if (values is IReadOnlyList<T> list)
        {
            return list.GetRandomItem();
        }

        T? result = default;
        var count = 0;

        foreach (var item in values)
        {
            count++;

            // Reservoir sampling with a sample size of 1
            if (RandomNumberGenerator.GetInt32(count) == 0)
            {
                result = item;
            }
        }

        return result;
    }

    /// <summary>
    ///     Generates random integers between a specified inclusive lower bound and a specified exclusive upper bound using a
    ///     cryptographically strong random number generator.
    /// </summary>
    /// <param name="count"></param>
    /// <param name="fromInclusive">The inclusive lower bound of the random range.</param>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns></returns>
    public static IEnumerable<int> RandomInts(this int count, int fromInclusive, int toExclusive)
    {
        for (var i = 0; i < count; i++)
        {
            yield return RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);
        }
    }

    /// <summary>
    ///     Generates a random integer between 0 (inclusive) and a specified exclusive upper bound using a cryptographically
    ///     strong random number generator.
    /// </summary>
    /// <param name="count"></param>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns></returns>
    public static IEnumerable<int> RandomInts(this int count, int toExclusive)
    {
        for (var i = 0; i < count; i++)
        {
            yield return RandomNumberGenerator.GetInt32(toExclusive);
        }
    }

    /// <summary>
    ///     Tries to convert a string value to an int32
    /// </summary>
    /// <param name="data"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int ToInt(this string? data, int defaultValue = 0)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return defaultValue;
        }

        return int.TryParse(data, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

#if !NET_6 && !NET_7
    /// <summary>
    ///     Performs an in-place shuffle of a span using cryptographically random number generation.
    /// </summary>
    /// <param name="values">The span to shuffle.</param>
    /// <typeparam name="T">The type of span.</typeparam>
    public static void Shuffle<T>(this Span<T> values) => RandomNumberGenerator.Shuffle(values);

    /// <summary>
    ///     Shuffles the elements of an array in-place using a cryptographically-secure random number generator.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="values">The array to shuffle.</param>
    public static void Shuffle<T>(this T[]? values)
    {
        if (values is null)
        {
            return;
        }

        RandomNumberGenerator.Shuffle(values.ToSpan());
    }

    /// <summary>
    ///     Shuffles the elements of a List of T in-place using a cryptographically-secure random number generator.
    ///     This is an efficient implementation using memory pinning.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    /// <param name="values">The list to shuffle.</param>
    public static void Shuffle<T>(this List<T>? values)
    {
        if (values is null)
        {
            return;
        }

        RandomNumberGenerator.Shuffle(values.ToSpan());
    }
#endif

#if !NET_6
    /// <summary>
    ///     Shuffles the elements of a collection implementing IList of T in-place using a cryptographically-secure random
    ///     number generator.
    ///     This implementation uses the Fisher-Yates shuffle algorithm.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    /// <param name="values">The list to shuffle.</param>
    public static void Shuffle<T>(this IList<T>? values)
    {
        if (values is null)
        {
            return;
        }

        var count = values.Count;

        while (count > 1)
        {
            count--;
            var k = RandomNumberGenerator.GetInt32(count + 1);
            (values[k], values[count]) = (values[count], values[k]); // Using tuple deconstruction for swapping
        }
    }

    /// <summary>
    ///     Determines if a value represents an integral number.
    /// </summary>
    public static bool IsInteger<T>(this T value)
        where T : INumber<T>
        => T.IsInteger(value);

    /// <summary>
    ///     Determines if a value represents a real number.
    /// </summary>
    public static bool IsRealNumber<T>(this T value)
        where T : INumber<T>
        => T.IsRealNumber(value);

    /// <summary>
    ///     Tries to convert a string value to a number
    /// </summary>
    public static T? ToNumber<T>(this string? data, NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands)
        where T : INumber<T>
    {
        var defaultValue = default(T);

        if (string.IsNullOrWhiteSpace(data))
        {
            return defaultValue;
        }

        return T.TryParse(data, style, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
    }

    /// <summary>
    ///     Converts Pounds to Kg
    /// </summary>
    public static T PoundsToKg<T>(T pounds)
        where T : INumber<T>
        => pounds * T.CreateChecked(value: 0.453592f);

    /// <summary>
    ///     Converts Inches to Cm
    /// </summary>
    public static T InchesToCm<T>(T inches)
        where T : INumber<T>
        => inches * T.CreateChecked(value: 2.54f);
#endif
}
