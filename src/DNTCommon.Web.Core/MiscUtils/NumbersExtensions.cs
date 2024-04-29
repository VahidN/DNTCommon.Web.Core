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

        return int.TryParse(data, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
    }
}