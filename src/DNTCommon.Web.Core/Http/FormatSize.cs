using System;
using System.Globalization;

namespace DNTCommon.Web.Core;

/// <summary>
/// File Size Formatter
/// </summary>
public static class FormatSize
{
    private static readonly string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    /// <summary>
    /// Returns the file's size in KB/MB format
    /// </summary>
    public static string ToFormattedFileSize(this int size)
    {
        return ToFormattedFileSize((long)size);
    }

    /// <summary>
    /// Returns the file's size in KB/MB format
    /// </summary>
    public static string ToFormattedFileSize(this long size)
    {
        const string formatTemplate = "{0}{1:0.#} {2}";

        if (size == 0)
        {
            return string.Format(CultureInfo.InvariantCulture, formatTemplate, null, 0, sizeSuffixes[0]);
        }

        var absSize = Math.Abs((double)size);
        var fpPower = Math.Log(absSize, 1000);
        var intPower = (int)fpPower;
        var iUnit = intPower >= sizeSuffixes.Length
            ? sizeSuffixes.Length - 1
            : intPower;
        var normSize = absSize / Math.Pow(1000, iUnit);

        return string.Format(
            CultureInfo.InvariantCulture,
            formatTemplate,
            size < 0 ? "-" : null, normSize, sizeSuffixes[iUnit]);
    }
}