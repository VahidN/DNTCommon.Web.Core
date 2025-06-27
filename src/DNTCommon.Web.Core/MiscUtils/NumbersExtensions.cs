using System.Numerics;

namespace DNTCommon.Web.Core;

/// <summary>
///     Converting string values to numbers
/// </summary>
public static class NumbersExtensions
{
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

        return int.TryParse(data, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

#if !NET_6

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
    public static T? ToNumber<T>(this string? data, NumberStyles style = NumberStyles.AllowDecimalPoint)
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
