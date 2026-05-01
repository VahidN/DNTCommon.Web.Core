using System.Numerics;

namespace DNTCommon.Web.Core;

/// <summary>
///     File Size Formatter
/// </summary>
public static class FormatSize
{
    private static readonly string[] SizeSuffixes = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];

    /// <summary>
    ///     Returns the file's size in KB/MB format
    /// </summary>
    public static string ToFormattedFileSize<T>(this T size)
        where T : INumber<T>
    {
        const string FormatTemplate = "{0}{1:0.#} {2}";

        if (size.IsZero())
        {
            return string.Format(CultureInfo.InvariantCulture, FormatTemplate, arg0: null, arg1: 0, SizeSuffixes[0]);
        }

        var absSize = T.Abs(size);
        var fpPower = Math.Log(double.CreateChecked(absSize), newBase: 1000);
        var intPower = (int)fpPower;
        var iUnit = intPower >= SizeSuffixes.Length ? SizeSuffixes.Length - 1 : intPower;
        var divisor = T.CreateChecked(value: 1000).Power(iUnit);
        var normSize = absSize / divisor;

        return string.Format(CultureInfo.InvariantCulture, FormatTemplate, size.IsNegative() ? "-" : null, normSize,
            SizeSuffixes[iUnit]);
    }
}
