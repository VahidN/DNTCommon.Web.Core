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
    public static string? NullIfEmptyOrWhiteSpace([NotNullIfNotNull(nameof(str))] this string? str)
        => !string.IsNullOrWhiteSpace(str) ? str : null;

    /// <summary>
    ///     Removes a substring from the start of the string
    /// </summary>
    public static string? TrimStart([NotNullIfNotNull(nameof(inputText))] this string? inputText,
        string value,
        StringComparison comparisonType)
    {
        if (string.IsNullOrEmpty(value))
        {
            return inputText;
        }

        while (!string.IsNullOrEmpty(inputText) && inputText.StartsWith(value, comparisonType))
        {
            inputText = inputText[value.Length..];
        }

        return inputText?.Trim();
    }

    /// <summary>
    ///     Removes a substring from the end of the string
    /// </summary>
    public static string? TrimEnd([NotNullIfNotNull(nameof(inputText))] this string? inputText,
        string value,
        StringComparison comparisonType)
    {
        if (string.IsNullOrEmpty(value))
        {
            return inputText;
        }

        while (!string.IsNullOrEmpty(inputText) && inputText.EndsWith(value, comparisonType))
        {
            inputText = inputText[..^value.Length];
        }

        return inputText?.Trim();
    }

    /// <summary>
    ///     Removes a substring from the end and start of the string
    /// </summary>
    public static string? Trim([NotNullIfNotNull(nameof(inputText))] this string? inputText,
        string value,
        StringComparison comparisonType)
        => TrimStart(TrimEnd(inputText, value, comparisonType), value, comparisonType);
}