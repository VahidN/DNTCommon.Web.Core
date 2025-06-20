#if !NET_6
using System.Numerics;

namespace DNTCommon.Web.Core;

public static class ComparisonsUtils
{
    /// <summary>
    ///     Asserts that the <paramref name="actual" /> floating point
    ///     is approximately equal to <paramref name="expected" /> considering an <paramref name="epsilon" />.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IFloatingPoint{TSelf}" />.</typeparam>
    public static bool IsApproximately<T>(this T actual, T expected, T epsilon)
        where T : IFloatingPoint<T>
        => T.Abs(actual - expected) <= epsilon * T.Max(T.One, T.Abs(expected));

    /// <summary>
    ///     default epsilon is 1e-6.
    /// </summary>
    public static bool IsApproximately<T>(this T actual, T expected)
        where T : IFloatingPoint<T>
        => actual.IsApproximately(expected, T.CreateChecked(value: 1e-6));

    /// <summary>
    ///     Asserts that the <paramref name="actual" /> numeric value is positive (greater than zero).
    /// </summary>
    public static bool IsPositive<T>(this T actual)
        where T : INumber<T>
        => actual.IsGreaterThan(T.Zero);

    /// <summary>
    ///     Asserts that the <paramref name="actual" /> numeric value is negative (less than zero).
    /// </summary>
    public static bool IsNegative<T>(this T actual)
        where T : INumber<T>
        => actual.IsSmallerThan(T.Zero);

    /// <summary>
    ///     Asserts that the <paramref name="actual" /> value is greater
    ///     than the given <paramref name="other" /> value.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsGreaterThan<T>(this T actual, T other)
        where T : IComparable<T>
        => actual.CompareTo(other) > 0;

    /// <summary>
    ///     Asserts that the <paramref name="actual" /> value is smaller
    ///     than the given <paramref name="other" /> value.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsSmallerThan<T>(this T actual, T other)
        where T : IComparable<T>
        => actual.CompareTo(other) < 0;

    /// <summary>
    ///     Asserts that the <paramref name="actual" /> value is greater or equal
    ///     the given <paramref name="other" /> value.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsAtLeast<T>(this T actual, T other)
        where T : IComparable<T>
        => actual.CompareTo(other) >= 0;

    /// <summary>
    ///     Asserts that the <paramref name="actual" /> value is smaller or equal
    ///     the given <paramref name="other" /> value.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsAtMost<T>(this T actual, T other)
        where T : IComparable<T>
        => actual.CompareTo(other) <= 0;

    /// <summary>
    ///     Asserts that the <paramref name="actual" /> value
    ///     is between <paramref name="min" /> and <paramref name="max" /> exclusive bounds.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsBetween<T>(this T actual, T min, T max)
        where T : IComparable<T>
        => actual.IsGreaterThan(min) && actual.IsSmallerThan(max);

    /// <summary>
    ///     Asserts that the <paramref name="actual" /> value
    ///     is between <paramref name="min" /> and <paramref name="max" /> inclusive bounds.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsInRange<T>(this T actual, T min, T max)
        where T : IComparable<T>
        => actual.IsAtLeast(min) && actual.IsAtMost(max);

    /// <summary>
    ///     Asserts that the <paramref name="actual" /> value is not between
    ///     the specified <paramref name="min" /> and <paramref name="max" /> exclusive bounds.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsNotBetween<T>(this T actual, T min, T max)
        where T : IComparable<T>
        => max.IsAtLeast(min) && (actual.CompareTo(min) <= 0 || actual.CompareTo(max) >= 0);

    /// <summary>
    ///     Asserts that the <paramref name="actual" /> value
    ///     is smaller than <paramref name="min" /> or greater than <paramref name="max" />.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsOutOfRange<T>(this T actual, T min, T max)
        where T : IComparable<T>
        => max.IsAtLeast(min) && (actual.CompareTo(min) < 0 || actual.CompareTo(max) > 0);

    /// <summary>
    ///     Asserts that the difference between two <see cref="DateTime" />
    ///     is within the specified <paramref name="tolerance" />.
    /// </summary>
    public static bool IsApproximately(this DateTime actual, DateTime expected, TimeSpan tolerance)
        => (actual - expected).Duration().IsAtMost(tolerance);

    /// <summary>
    ///     Asserts that the difference between two <see cref="TimeSpan" />
    ///     is within the specified <paramref name="tolerance" />.
    /// </summary>
    public static bool IsApproximately(this TimeSpan actual, TimeSpan expected, TimeSpan tolerance)
        => (actual - expected).Duration().IsAtMost(tolerance);
}
#endif
