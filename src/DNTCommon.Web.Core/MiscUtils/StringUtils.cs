using System.Runtime.CompilerServices;

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
        => data.IsEmpty() ? [] : data.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries).ToList();

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

    /// <summary>
    ///     Determines whether two specified String objects have the same value.
    /// </summary>
    public static bool AreNullOrEmptyOrEqual([NotNullWhen(returnValue: false)] this string? item1,
        [NotNullWhen(returnValue: false)] string? item2,
        StringComparison comparisonType)
        => item1.IsEmpty() || item2.IsEmpty() || string.Equals(item1, item2, comparisonType);

    /// <summary>
    ///     Indicates whether this string is null or an Empty string.
    /// </summary>
    public static bool IsNullOrEmpty([NotNullWhen(returnValue: false)] this string? str) => string.IsNullOrEmpty(str);

    /// <summary>
    ///     Indicates whether this string is null, empty, or consists only of white-space characters.
    /// </summary>
    public static bool IsNullOrWhiteSpace([NotNullWhen(returnValue: false)] this string? str)
        => string.IsNullOrWhiteSpace(str);

    /// <summary>
    ///     Indicates whether this string IsNullOrEmpty or IsNullOrWhiteSpace.
    /// </summary>
    public static bool IsEmpty([NotNullWhen(returnValue: false)] this string? value)
        => string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);

    /// <summary>
    ///     Uses string.Split method to split given string by a given separator.
    /// </summary>
    public static string[]? Split([NotNullIfNotNull(nameof(str))] this string? str, string separator)
        => str?.Split([separator], StringSplitOptions.RemoveEmptyEntries);

    /// <summary>
    ///     Uses string.Split method to split given string by a given separator.
    /// </summary>
    public static string[]? Split([NotNullIfNotNull(nameof(str))] this string? str, char separator)
        => str?.Split(separator, StringSplitOptions.RemoveEmptyEntries);

    /// <summary>
    ///     Uses string.Split method to split given string by a given separator.
    /// </summary>
    public static string[]? Split([NotNullIfNotNull(nameof(str))] this string? str,
        string separator,
        StringSplitOptions options)
        => str?.Split([separator], options);

    /// <summary>
    ///     Uses string.Split method to split given string by a given separator.
    /// </summary>
    public static string[]? Split([NotNullIfNotNull(nameof(str))] this string? str,
        char separator,
        StringSplitOptions options)
        => str?.Split(separator, options);

    /// <summary>
    ///     Converts string to enum value.
    /// </summary>
    public static T ToEnum<T>([NotNullIfNotNull(nameof(value))] this string? value,
        T defaultValue = default,
        bool ignoreCase = true)
        where T : struct
    {
        if (value.IsEmpty())
        {
            return defaultValue;
        }

        return Enum.TryParse<T>(value, ignoreCase, out var result) ? result : defaultValue;
    }

    /// <summary>
    ///     Converts line endings in the string to <see cref="Environment.NewLine" />.
    /// </summary>
    public static string? NormalizeLineEndings([NotNullIfNotNull(nameof(text))] this string? text)
        => text?.Replace(oldValue: "\r\n", newValue: "\n", StringComparison.InvariantCulture)
            .Replace(oldValue: "\r", newValue: "\n", StringComparison.InvariantCulture)
            .Replace(oldValue: "\n", Environment.NewLine, StringComparison.InvariantCulture);

    /// <summary>
    ///     Formats a string to an invariant culture
    /// </summary>
    public static string ToInvariantString(this ref DefaultInterpolatedStringHandler handler)
        => string.Create(CultureInfo.InvariantCulture, ref handler);
}