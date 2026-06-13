using System.Numerics;

namespace DNTCommon.Web.Core;

/// <summary>
///     File Size Formatter
/// </summary>
public static class FormatSize
{
    private const int BinaryDivisor = 1024;

    private static readonly string[] SizeSuffixes = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];

    /// <summary>
    ///     Returns the file size formatted as B/KB/MB/GB...
    ///     If you need more control and precision, change the formatTemplate to `{0}{1:0.##} {2}`
    /// </summary>
    public static string ToFormattedFileSize<T>(this T size, string formatTemplate = "{0}{1:0.#} {2}")
        where T : INumber<T>
    {
        if (size.IsZero())
        {
            return string.Format(CultureInfo.InvariantCulture, formatTemplate, arg0: null, arg1: 0, SizeSuffixes[0]);
        }

        var absSize = T.Abs(size);

        var value = double.CreateChecked(absSize);

        var unitIndex = 0;

        while (value >= BinaryDivisor && unitIndex < SizeSuffixes.Length - 1)
        {
            value /= BinaryDivisor;
            unitIndex++;
        }

        return string.Format(CultureInfo.InvariantCulture, formatTemplate, size.IsNegative() ? "-" : null, value,
            SizeSuffixes[unitIndex]);
    }
}
