namespace DNTCommon.Web.Core;

/// <summary>
///     Provides methods for generating cryptographically-strong random numbers.
/// </summary>
public sealed class RandomNumberProvider : IRandomNumberProvider
{
    /// <summary>
    ///     Fills an array of bytes with a cryptographically strong random sequence of values.
    /// </summary>
    /// <param name="randomBytes"></param>
    public void NextBytes(byte[] randomBytes) => RandomNumberGenerator.Fill(randomBytes);

    /// <summary>
    ///     Generates a random non-negative number.
    /// </summary>
    public int NextNumber()
    {
        var randb = new byte[4];
        RandomNumberGenerator.Fill(randb);
        var value = BitConverter.ToInt32(randb, startIndex: 0);

        if (value < 0)
        {
            value = -value;
        }

        return value;
    }

    /// <summary>
    ///     Generates a random non-negative number less than or equal to max.
    /// </summary>
    /// <param name="max">The maximum possible value.</param>
    public int NextNumber(int max)
    {
        var randb = new byte[4];
        RandomNumberGenerator.Fill(randb);
        var value = BitConverter.ToInt32(randb, startIndex: 0);
        value %= max + 1; // % calculates remainder

        if (value < 0)
        {
            value = -value;
        }

        return value;
    }

    /// <summary>
    ///     Generates a random non-negative number bigger than or equal to min and less than or
    ///     equal to max.
    /// </summary>
    /// <param name="min">The minimum possible value.</param>
    /// <param name="max">The maximum possible value.</param>
    public int NextNumber(int min, int max)
    {
        var value = NextNumber(max - min) + min;

        return value;
    }

    /// <summary>
    ///     Returns a random item of from a give Enum
    /// </summary>
    public TEnum GetRandomEnumItem<TEnum>()
        where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>();

        return values[RandomNumberGenerator.GetInt32(values.Length)];
    }

    /// <summary>
    ///     Generates a random integer between 0 (inclusive) and a specified exclusive upper bound using a
    ///     cryptographically strong random number generator.
    /// </summary>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <exception>The <paramref name="toExclusive" /> parameter is less than or equal to 0.</exception>
    /// <returns>A random integer between 0 (inclusive) and <paramref name="toExclusive" /> (exclusive).</returns>
    public int GetSecureRandomInt32(int toExclusive) => RandomNumberGenerator.GetInt32(toExclusive);

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
    public int GetSecureRandomInt32(int fromInclusive, int toExclusive)
        => RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);
}
