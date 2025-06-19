#if !NET_6
using System.Numerics;

namespace DNTCommon.Web.Core;

/// <summary>
///     File Size Units Extensions
/// </summary>
public static class FileSizeUnitsExtensions
{
    /// <summary>
    ///     Converts the given size to bytes
    /// </summary>
    public static T ToBytes<T>(this T size, FileSizeUnit fromUnit)
        where T : INumber<T>
        => fromUnit switch
        {
            FileSizeUnit.Byte => size,
            FileSizeUnit.Kilobyte => size.KilobytesToBytes(),
            FileSizeUnit.Megabyte => size.MegabytesToBytes(),
            FileSizeUnit.Gigabyte => size.GigabytesToBytes(),
            FileSizeUnit.Terabyte => size.TerabytesToBytes(),
            _ => T.Zero
        };

    /// <summary>
    ///     Converts the given size to Kilobytes
    /// </summary>
    public static T ToKilobytes<T>(this T size, FileSizeUnit fromUnit)
        where T : INumber<T>
        => fromUnit switch
        {
            FileSizeUnit.Byte => size.BytesToKilobytes(),
            FileSizeUnit.Kilobyte => size,
            FileSizeUnit.Megabyte => size.MegabytesToKilobytes(),
            FileSizeUnit.Gigabyte => size.GigabytesToKilobytes(),
            FileSizeUnit.Terabyte => size.TerabytesToKilobytes(),
            _ => T.Zero
        };

    /// <summary>
    ///     Converts the given size to Megabytes
    /// </summary>
    public static T ToMegabytes<T>(this T size, FileSizeUnit fromUnit)
        where T : INumber<T>
        => fromUnit switch
        {
            FileSizeUnit.Byte => size.BytesToMegabytes(),
            FileSizeUnit.Kilobyte => size.KilobytesToMegabytes(),
            FileSizeUnit.Megabyte => size,
            FileSizeUnit.Gigabyte => size.GigabytesToMegabytes(),
            FileSizeUnit.Terabyte => size.TerabytesToMegabytes(),
            _ => T.Zero
        };

    /// <summary>
    ///     Converts the given size to Gigabytes
    /// </summary>
    public static T ToGigabytes<T>(this T size, FileSizeUnit fromUnit)
        where T : INumber<T>
        => fromUnit switch
        {
            FileSizeUnit.Byte => size.BytesToGigabytes(),
            FileSizeUnit.Kilobyte => size.KilobytesToGigabytes(),
            FileSizeUnit.Megabyte => size.MegabytesToGigabytes(),
            FileSizeUnit.Gigabyte => size,
            FileSizeUnit.Terabyte => size.TerabytesToGigabytes(),
            _ => T.Zero
        };

    /// <summary>
    ///     Converts the given size to Terabytes
    /// </summary>
    public static T ToTerabytes<T>(this T size, FileSizeUnit fromUnit)
        where T : INumber<T>
        => fromUnit switch
        {
            FileSizeUnit.Byte => size.BytesToTerabytes(),
            FileSizeUnit.Kilobyte => size.KilobytesToTerabytes(),
            FileSizeUnit.Megabyte => size.MegabytesToTerabytes(),
            FileSizeUnit.Gigabyte => size.GigabytesToTerabytes(),
            FileSizeUnit.Terabyte => size,
            _ => T.Zero
        };

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T BytesTo<T>(this T size, FileSizeUnit toUnit)
        where T : INumber<T>
        => toUnit switch
        {
            FileSizeUnit.Byte => size,
            FileSizeUnit.Kilobyte => size.BytesToKilobytes(),
            FileSizeUnit.Megabyte => size.BytesToMegabytes(),
            FileSizeUnit.Gigabyte => size.BytesToGigabytes(),
            FileSizeUnit.Terabyte => size.BytesToTerabytes(),
            _ => T.Zero
        };

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T KilobytesTo<T>(this T size, FileSizeUnit toUnit)
        where T : INumber<T>
        => toUnit switch
        {
            FileSizeUnit.Byte => size.KilobytesToBytes(),
            FileSizeUnit.Kilobyte => size,
            FileSizeUnit.Megabyte => size.KilobytesToMegabytes(),
            FileSizeUnit.Gigabyte => size.KilobytesToGigabytes(),
            FileSizeUnit.Terabyte => size.KilobytesToTerabytes(),
            _ => T.Zero
        };

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T MegabytesTo<T>(this T size, FileSizeUnit toUnit)
        where T : INumber<T>
        => toUnit switch
        {
            FileSizeUnit.Byte => size.MegabytesToBytes(),
            FileSizeUnit.Kilobyte => size.MegabytesToKilobytes(),
            FileSizeUnit.Megabyte => size,
            FileSizeUnit.Gigabyte => size.MegabytesToGigabytes(),
            FileSizeUnit.Terabyte => size.MegabytesToTerabytes(),
            _ => T.Zero
        };

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T GigabytesTo<T>(this T size, FileSizeUnit toUnit)
        where T : INumber<T>
        => toUnit switch
        {
            FileSizeUnit.Byte => size.GigabytesToBytes(),
            FileSizeUnit.Kilobyte => size.GigabytesToKilobytes(),
            FileSizeUnit.Megabyte => size.GigabytesToMegabytes(),
            FileSizeUnit.Gigabyte => size,
            FileSizeUnit.Terabyte => size.GigabytesToTerabytes(),
            _ => T.Zero
        };

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T TerabytesTo<T>(this T size, FileSizeUnit toUnit)
        where T : INumber<T>
        => toUnit switch
        {
            FileSizeUnit.Byte => size.TerabytesToBytes(),
            FileSizeUnit.Kilobyte => size.TerabytesToKilobytes(),
            FileSizeUnit.Megabyte => size.TerabytesToMegabytes(),
            FileSizeUnit.Gigabyte => size.TerabytesToGigabytes(),
            FileSizeUnit.Terabyte => size,
            _ => T.Zero
        };

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T BytesToKilobytes<T>(this T size)
        where T : INumber<T>
        => size.DivideBy1024();

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T BytesToMegabytes<T>(this T size)
        where T : INumber<T>
        => size.DivideBy1024(times: 2);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T BytesToGigabytes<T>(this T size)
        where T : INumber<T>
        => size.DivideBy1024(times: 3);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T BytesToTerabytes<T>(this T size)
        where T : INumber<T>
        => size.DivideBy1024(times: 4);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T KilobytesToBytes<T>(this T size)
        where T : INumber<T>
        => size.MultiplyBy1024();

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T KilobytesToMegabytes<T>(this T size)
        where T : INumber<T>
        => size.DivideBy1024();

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T KilobytesToGigabytes<T>(this T size)
        where T : INumber<T>
        => size.DivideBy1024(times: 2);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T KilobytesToTerabytes<T>(this T size)
        where T : INumber<T>
        => size.DivideBy1024(times: 3);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T MegabytesToBytes<T>(this T size)
        where T : INumber<T>
        => size.MultiplyBy1024(times: 2);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T MegabytesToKilobytes<T>(this T size)
        where T : INumber<T>
        => size.MultiplyBy1024(times: 1);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T MegabytesToGigabytes<T>(this T size)
        where T : INumber<T>
        => size.DivideBy1024();

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T MegabytesToTerabytes<T>(this T size)
        where T : INumber<T>
        => size.DivideBy1024(times: 2);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T GigabytesToBytes<T>(this T size)
        where T : INumber<T>
        => size.MultiplyBy1024(times: 3);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T GigabytesToKilobytes<T>(this T size)
        where T : INumber<T>
        => size.MultiplyBy1024(times: 2);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T GigabytesToMegabytes<T>(this T size)
        where T : INumber<T>
        => size.MultiplyBy1024();

    public static T GigabytesToTerabytes<T>(this T size)
        where T : INumber<T>
        => size.DivideBy1024();

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T TerabytesToBytes<T>(this T size)
        where T : INumber<T>
        => size.MultiplyBy1024(times: 4);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T TerabytesToKilobytes<T>(this T size)
        where T : INumber<T>
        => size.MultiplyBy1024(times: 3);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T TerabytesToMegabytes<T>(this T size)
        where T : INumber<T>
        => size.MultiplyBy1024(times: 2);

    /// <summary>
    ///     Converts the given number to the specified unit
    /// </summary>
    public static T TerabytesToGigabytes<T>(this T size)
        where T : INumber<T>
        => size.MultiplyBy1024();

    /// <summary>
    ///     Multiples the given number by 1024
    /// </summary>
    public static T MultiplyBy1024<T>(this T number, byte times = 1)
        where T : INumber<T>
    {
        var divisorBase = T.CreateChecked(value: 1024);
        var divisor = Power(divisorBase, times);

        return number * divisor;
    }

    /// <summary>
    ///     Divides the given number by 1024
    /// </summary>
    public static T DivideBy1024<T>(this T number, byte times = 1)
        where T : INumber<T>
    {
        var divisorBase = T.CreateChecked(value: 1024);
        var divisor = Power(divisorBase, times);

        return number / divisor;
    }

    /// <summary>
    ///     Returns a specified number raised to the specified power.
    /// </summary>
    public static T Power<T>(this T baseValue, byte exponent)
        where T : INumber<T>
    {
        if (exponent == 0)
        {
            return T.One;
        }

        var result = baseValue;

        for (var i = 1; i < exponent; i++)
        {
            result *= baseValue;
        }

        return result;
    }
}
#endif
