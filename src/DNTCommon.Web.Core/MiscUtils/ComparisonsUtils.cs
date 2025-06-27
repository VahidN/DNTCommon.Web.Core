#if !NET_6
using System.Numerics;

namespace DNTCommon.Web.Core;

public static class ComparisonsUtils
{
    /// <summary>
    ///     Asserts that the <paramref name="value" /> floating point
    ///     is approximately equal to <paramref name="other" /> considering an <paramref name="epsilon" />.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IFloatingPoint{TSelf}" />.</typeparam>
    public static bool IsApproximately<T>(this T value, T other, T epsilon)
        where T : IFloatingPoint<T>
        => T.Abs(value - other) <= epsilon * T.Max(T.One, T.Abs(other));

    /// <summary>
    ///     default epsilon is 1e-6.
    /// </summary>
    public static bool IsApproximately<T>(this T value, T other)
        where T : IFloatingPoint<T>
        => value.IsApproximately(other, T.CreateChecked(value: 1e-6));

    /// <summary>
    ///     Asserts that the <paramref name="value" /> numeric value is positive (greater than zero).
    /// </summary>
    public static bool IsPositive<T>(this T value)
        where T : INumber<T>
        => value.IsGreaterThan(T.Zero);

    /// <summary>
    ///     Asserts that the <paramref name="value" /> numeric value is negative (less than zero).
    /// </summary>
    public static bool IsNegative<T>(this T value)
        where T : INumber<T>
        => value.IsSmallerThan(T.Zero);

    /// <summary>
    ///     Asserts that the <paramref name="value" /> value is greater
    ///     than the given <paramref name="other" /> value.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsGreaterThan<T>(this T value, T other)
        where T : IComparable<T>
        => value.CompareTo(other) > 0;

    /// <summary>
    ///     Asserts that the <paramref name="value" /> value is smaller
    ///     than the given <paramref name="other" /> value.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsSmallerThan<T>(this T value, T other)
        where T : IComparable<T>
        => value.CompareTo(other) < 0;

    /// <summary>
    ///     Asserts that the <paramref name="value" /> value is greater or equal
    ///     the given <paramref name="other" /> value.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsAtLeast<T>(this T value, T other)
        where T : IComparable<T>
        => value.CompareTo(other) >= 0;

    /// <summary>
    ///     Asserts that the <paramref name="value" /> value is smaller or equal
    ///     the given <paramref name="other" /> value.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsAtMost<T>(this T value, T other)
        where T : IComparable<T>
        => value.CompareTo(other) <= 0;

    /// <summary>
    ///     Asserts that the <paramref name="value" /> value
    ///     is between <paramref name="min" /> and <paramref name="max" /> exclusive bounds.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsBetween<T>(this T value, T min, T max)
        where T : IComparable<T>
        => value.IsGreaterThan(min) && value.IsSmallerThan(max);

    /// <summary>
    ///     Asserts that the <paramref name="value" /> value
    ///     is between <paramref name="min" /> and <paramref name="max" /> inclusive bounds.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsInRange<T>(this T value, T min, T max)
        where T : IComparable<T>
        => value.IsAtLeast(min) && value.IsAtMost(max);

    /// <summary>
    ///     Asserts that the <paramref name="value" /> value is not between
    ///     the specified <paramref name="min" /> and <paramref name="max" /> exclusive bounds.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsNotBetween<T>(this T value, T min, T max)
        where T : IComparable<T>
        => max.IsAtLeast(min) && (value.CompareTo(min) <= 0 || value.CompareTo(max) >= 0);

    /// <summary>
    ///     Asserts that the <paramref name="value" /> value
    ///     is smaller than <paramref name="min" /> or greater than <paramref name="max" />.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable" />.</typeparam>
    public static bool IsOutOfRange<T>(this T value, T min, T max)
        where T : IComparable<T>
        => max.IsAtLeast(min) && (value.CompareTo(min) < 0 || value.CompareTo(max) > 0);

    /// <summary>
    ///     Asserts that the difference between two <see cref="DateTime" />
    ///     is within the specified <paramref name="tolerance" />.
    /// </summary>
    public static bool IsApproximately(this DateTime value, DateTime other, TimeSpan tolerance)
        => (value - other).Duration().IsAtMost(tolerance);

    /// <summary>
    ///     Asserts that the difference between two <see cref="TimeSpan" />
    ///     is within the specified <paramref name="tolerance" />.
    /// </summary>
    public static bool IsApproximately(this TimeSpan value, TimeSpan other, TimeSpan tolerance)
        => (value - other).Duration().IsAtMost(tolerance);
}
#endif
