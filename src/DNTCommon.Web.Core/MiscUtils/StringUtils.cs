namespace DNTCommon.Web.Core;

/// <summary>
///     String Utils
/// </summary>
public static class StringUtils
{
    /// <summary>
    ///     Converts a  multi-line text to a list. It's useful for textarea to db field value binding.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static IList<string> ConvertMultiLineTextToList(this string? data)
        => string.IsNullOrWhiteSpace(data)
            ? []
            : data.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries).ToList();

    /// <summary>
    ///     Converts a list to a multi-line text. It's useful for db field values to textarea binding.
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static string ConvertListToMultiLineText(this ICollection<string>? items)
        => items == null || items.Count == 0 ? string.Empty : string.Join(Environment.NewLine, items);

    /// <summary>
    ///     Simplified version of IsNullOrWhiteSpace
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string? NullIfEmptyOrWhiteSpace(this string? str) => !string.IsNullOrWhiteSpace(str) ? str : null;
}