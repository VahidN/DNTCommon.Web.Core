namespace DNTCommon.Web.Core;

/// <summary>
///     Provides methods for generating cryptographically-strong random numbers.
/// </summary>
public interface IRandomNumberProvider
{
    /// <summary>
    ///     Fills an array of bytes with a cryptographically strong random sequence of values.
    /// </summary>
    /// <param name="randomBytes"></param>
    void NextBytes(byte[] randomBytes);

    /// <summary>
    ///     Generates a random non-negative number.
    /// </summary>
    int NextNumber();

    /// <summary>
    ///     Generates a random non-negative number less than or equal to max.
    /// </summary>
    /// <param name="max">The maximum possible value.</param>
    int NextNumber(int max);

    /// <summary>
    ///     Generates a random non-negative number bigger than or equal to min and less than or
    ///     equal to max.
    /// </summary>
    /// <param name="min">The minimum possible value.</param>
    /// <param name="max">The maximum possible value.</param>
    int NextNumber(int min, int max);

    /// <summary>
    ///     Returns a random item of from a give Enum
    /// </summary>
    TEnum GetRandomEnumItem<TEnum>()
        where TEnum : struct, Enum;

    /// <summary>
    ///     Generates a random integer between 0 (inclusive) and a specified exclusive upper bound using a
    ///     cryptographically strong random number generator.
    /// </summary>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <exception>The <paramref name="toExclusive" /> parameter is less than or equal to 0.</exception>
    /// <returns>A random integer between 0 (inclusive) and <paramref name="toExclusive" /> (exclusive).</returns>
    int GetSecureRandomInt32(int toExclusive);

    /// <summary>
    ///     Generates a random integer between a specified inclusive lower bound and a specified exclusive upper bound
    ///     using a cryptographically strong random number generator.
    /// </summary>
    /// <param name="fromInclusive">The inclusive lower bound of the random range.</param>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <exception>
    ///     The <paramref name="toExclusive" /> parameter is less than or equal to the <paramref name="fromInclusive" />
    ///     parameter.
    /// </exception>
    /// <returns>
    ///     A random integer between <paramref name="fromInclusive" /> (inclusive) and <paramref name="toExclusive" />
    ///     (exclusive).
    /// </returns>
    int GetSecureRandomInt32(int fromInclusive, int toExclusive);
}
