using System.Runtime.CompilerServices;
using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     String Utils
/// </summary>
public static class StringUtils
{
    private static readonly char[] SmallCapitalLetters =
    [
        'ᴀ', 'ʙ', 'ᴄ', 'ᴅ', 'ᴇ', 'ғ', 'ɢ', 'ʜ', 'ɪ', 'ᴊ', 'ᴋ', 'ʟ', 'ᴍ', 'ɴ', 'ᴏ', 'ᴘ', 'ʀ', 'ᴛ', 'ᴜ', 'ᴡ', 'ʏ', 'ᴢ'
    ];

    /// <summary>
    ///     Converts the specified string to title case (except for words that are entirely in uppercase, which are considered
    ///     to be acronyms).
    /// </summary>
    public static string? ToTitleCase([NotNullIfNotNull(nameof(input))] string? input, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CultureInfo.InvariantCulture;

        return input.IsEmpty() ? null : cultureInfo.TextInfo.ToTitleCase(input);
    }

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
        => items is null || items.Count == 0 ? string.Empty : string.Join(Environment.NewLine, items);

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
        => text?.Replace(oldValue: "\r\n", newValue: "\n", StringComparison.Ordinal)
            .Replace(oldChar: '\r', newChar: '\n')
            .Replace(oldValue: "\n", Environment.NewLine, StringComparison.Ordinal);

    /// <summary>
    ///     Formats a string to an invariant culture
    /// </summary>
    public static string ToInvariantString(this ref DefaultInterpolatedStringHandler handler)
        => string.Create(CultureInfo.InvariantCulture, ref handler);

    /// <summary>
    ///     Some characters such as `ｅｘｐｒｅｓｓｉｏｎ` are called "full-width" characters.
    ///     They look like regular letters but are actually different Unicode code points.
    /// </summary>
    public static bool IsFullWidthChar(this char c)
        => c is >= (char)0x1100 and <= (char)0x115F or // Hangul Jamo
            >= (char)0x2E80 and <= (char)0xA4CF and not (char)0x303F or // CJK Radicals Supplement and Kangxi Radicals
            >= (char)0xAC00 and <= (char)0xD7A3 or // Hangul Syllables
            >= (char)0xF900 and <= (char)0xFAFF or // CJK Compatibility Ideographs
            >= (char)0xFE10 and <= (char)0xFE19 or // Vertical forms
            >= (char)0xFE30 and <= (char)0xFE6F or // CJK Compatibility Forms
            >= (char)0xFF00 and <= (char)0xFF60 or // Fullwidth Forms
            >= (char)0xFFE0 and <= (char)0xFFE6; // Fullwidth Symbol Forms

    /// <summary>
    ///     Some characters such as `ｅｘｐｒｅｓｓｉｏｎ` are called "full-width" characters.
    ///     They look like regular letters but are actually different Unicode code points.
    /// </summary>
    public static bool HasFullWidthChar(this string? input)
        => !input.IsEmpty() && input.ToCharArray().Any(chr => chr.IsFullWidthChar());

    /// <summary>
    ///     Some characters such as `ｅｘｐｒｅｓｓｉｏｎ` are called "full-width" characters.
    ///     They look like regular letters but are actually different Unicode code points.
    /// </summary>
    public static bool IsFullWidthCharString(this string? input)
        => !input.IsEmpty() && input.ToCharArray().All(chr => chr.IsFullWidthChar());

    /// <summary>
    ///     The lowercase small capital I is similar in size to the letter "i" but is shaped like the capital letter "I".
    /// </summary>
    public static bool IsSmallCapitalChar(this char c) => SmallCapitalLetters.Contains(c);

    /// <summary>
    ///     The lowercase small capital I is similar in size to the letter "i" but is shaped like the capital letter "I".
    /// </summary>
    public static bool HasSmallCapitalChar(this string? input)
        => !input.IsEmpty() && input.ToCharArray().Any(chr => chr.IsSmallCapitalChar());

    /// <summary>
    ///     The lowercase small capital I is similar in size to the letter "i" but is shaped like the capital letter "I".
    /// </summary>
    public static bool IsSmallCapitalCharString(this string? input)
        => !input.IsEmpty() && input.ToCharArray().All(chr => chr.IsSmallCapitalChar());

    /// <summary>
    ///     Determines whether the given text contains any of the provided values, using the specified comparison rules.
    /// </summary>
    public static bool ContainsOneOfValues(this string? text, StringComparison comparisonType, params string[]? values)
        => values?.Any(value => text?.Contains(value, comparisonType) == true) == true;

    /// <summary>
    ///     Determines whether the given text contains all the provided values, using the specified comparison rules.
    /// </summary>
    public static bool ContainsAllValues(this string? text, StringComparison comparisonType, params string[]? values)
        => values?.All(value => text?.Contains(value, comparisonType) == true) == true;

    /// <summary>
    ///     Determines whether all characters of the given text are unique
    /// </summary>
    public static bool HasUniqueChars(this string? text) => text?.Distinct().Count() == text?.Length;

    /// <summary>
    ///     Determines whether the given text has N spaces in it
    /// </summary>
    public static bool HasSpaces(this string? text, int count = 1) => text?.Count(c => c == ' ') == count;

    /// <summary>
    ///     Determines whether the given text has any space in it
    /// </summary>
    public static bool HasAnySpace(this string? text) => text?.Any(c => c == ' ') == true;

    /// <summary>
    ///     Determines whether all characters of the given text are similar
    /// </summary>
    public static bool HasConsecutiveChars(this string? text) => text?.Distinct().Count() == 1;

    /// <summary>
    ///     Determines whether N characters of the given text are similar
    /// </summary>
    public static bool HasNConsecutiveChars(this string inputText, int sequenceLength = 3)
    {
        var charEnumerator = StringInfo.GetTextElementEnumerator(inputText);
        var currentElement = string.Empty;
        var count = 1;

        while (charEnumerator.MoveNext())
        {
            if (string.Equals(currentElement, charEnumerator.GetTextElement(), StringComparison.Ordinal))
            {
                if (++count >= sequenceLength)
                {
                    return true;
                }
            }
            else
            {
                count = 1;
                currentElement = charEnumerator.GetTextElement();
            }
        }

        return false;
    }

    /// <summary>
    ///     Concatenates the given elements
    /// </summary>
    public static string? ConcatWith(this string? text, params IEnumerable<string>? items)
    {
        if (items is null)
        {
            return text;
        }

        if (text is null)
        {
            return string.Concat(items);
        }

        string?[] strings = [text, ..items];

        return string.Concat(strings);
    }

    /// <summary>
    ///     Concatenates the given elements
    /// </summary>
    public static string? ConcatItems([NotNullIfNotNull(nameof(items))] this IEnumerable<string>? items)
        => items is null ? null : string.Concat(items);

    /// <summary>
    ///     Concatenates the given elements
    /// </summary>
    public static string? ConcatItems<T>([NotNullIfNotNull(nameof(items))] this IEnumerable<T>? items,
        Func<T, string> toStringFunc)
    {
        ArgumentNullException.ThrowIfNull(toStringFunc);

        if (items is null)
        {
            return null;
        }

        var sb = new StringBuilder();

        foreach (var item in items)
        {
            sb.Append(toStringFunc(item));
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Returns a value indicating whether a specified character occurs within this string, using the specified comparison
    ///     rules.
    /// </summary>
    public static bool ContainsChar(this string? text, char value, bool ignoreCase = false)
        => ignoreCase
            ? text?.Contains(value, StringComparison.OrdinalIgnoreCase) == true
            : text?.Contains(value, StringComparison.Ordinal) == true;

    /// <summary>
    ///     Determines whether two specified String objects have the same value. A parameter specifies the culture, case, and
    ///     sort rules used in the comparison.
    /// </summary>
    public static bool EqualsTo(this string? a, string? b, StringComparison comparison = StringComparison.Ordinal)
        => string.Equals(a, b, comparison);

    /// <summary>
    ///     Reports the zero-based indexes of the occurrences of the specified string in the current String object.
    /// </summary>
    public static IEnumerable<int> GetAllIndexesOf(this string? text, string? subString, bool ignoreCase = false)
    {
        if (text is null || subString is null)
        {
            yield break;
        }

        var index = 0;

        while ((index = text.IndexOf(subString, index,
                   ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) != -1)
        {
            yield return index++;
        }
    }

    /// <summary>
    ///     Tries to return the first character of the given input
    /// </summary>
    public static string? GetFirstChar([NotNullIfNotNull(nameof(input))] this string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        return input.Length >= 1 ? input[index: 0].ToString() : input;
    }

    /// <summary>
    ///     Tries to return the last character of the given input
    /// </summary>
    public static string? GetLastChar([NotNullIfNotNull(nameof(input))] this string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        return input.Length >= 1 ? input[^1].ToString() : input;
    }

    /// <summary>
    ///     Returns a sub-str after the given value in the provided input
    /// </summary>
    public static string? SubStrAfter([NotNullIfNotNull(nameof(input))] this string? input,
        [NotNullIfNotNull(nameof(value))] string? value,
        StringComparison comparison = StringComparison.Ordinal)
    {
        if (value is null || input is null)
        {
            return null;
        }

        var indexOf = input.IndexOf(value, comparison);

        return indexOf == -1 ? null : input[(indexOf + value.Length)..];
    }

    /// <summary>
    ///     Returns a sub-str before the given value in the provided input
    /// </summary>
    public static string? SubStrBefore([NotNullIfNotNull(nameof(input))] this string? input,
        [NotNullIfNotNull(nameof(value))] string? value,
        StringComparison comparison = StringComparison.Ordinal)
    {
        if (value is null || input is null)
        {
            return null;
        }

        var indexOf = input.IndexOf(value, comparison);

        return indexOf == -1 ? null : input[..indexOf];
    }

    /// <summary>
    ///     Concatenates the members of a constructed IEnumerable collection of type String, using the specified separator
    ///     between each member.
    /// </summary>
    public static string? Join([NotNullIfNotNull(nameof(values))] this IEnumerable<string?>? values, string? separator)
        => values is null ? null : string.Join(separator, values);

    /// <summary>
    ///     Tries to take n chars from the beginning of the given string safely
    /// </summary>
    public static string? TakeFirstNChars([NotNullIfNotNull(nameof(input))] this string? input, int length)
        => input?[..Math.Min(length, input.Length)];

    /// <summary>
    ///     Tries to take n chars from the end of the given string safely
    /// </summary>
    public static string? TakeLastNChars([NotNullIfNotNull(nameof(input))] this string? input, int length)
        => input?[^Math.Min(length, input.Length)..];

    /// <summary>
    ///     Tries to remove n chars from the beginning of the given string safely
    /// </summary>
    public static string? RemoveFirstNChars([NotNullIfNotNull(nameof(input))] this string? input, int length)
        => input?[Math.Min(length, input.Length)..];

    /// <summary>
    ///     Tries to remove n chars from the end of the given string safely
    /// </summary>
    public static string? RemoveLastNChars([NotNullIfNotNull(nameof(input))] this string? input, int length)
        => input?[..^Math.Min(length, input.Length)];

    /// <summary>
    ///     Reads lines of characters from the current string and returns the data as a string.
    /// </summary>
    public static IEnumerable<string> GetLines(this string? text)
    {
        if (text is null)
        {
            yield break;
        }

        using var stringReader = new StringReader(text);

        while (stringReader.ReadLine() is { } line)
        {
            yield return line;
        }
    }

    /// <summary>
    ///     Returns a new string in which all occurrences of specified values are replaced with "".
    /// </summary>
    public static string? RemoveAllValues([NotNullIfNotNull(nameof(text))] this string? text,
        StringComparison comparison,
        params ICollection<string>? values)
    {
        if (text is null)
        {
            return null;
        }

        if (values is null || values.Count == 0)
        {
            return text;
        }

        return values.Aggregate(text, (current, value) => current.Replace(value, string.Empty, comparison));
    }

    /// <summary>
    ///     Inverts the order of the elements in a sequence.
    /// </summary>
    public static string? ReverseChars([NotNullIfNotNull(nameof(input))] this string? input)
        => input is null ? null : string.Concat(input.Reverse());

    /// <summary>
    ///     Counts words in the given text based on the available white-spaces.
    /// </summary>
    public static int CountWordsInText(this string? text) => text is null ? 0 : CountWordsInText(text.AsSpan());

    /// <summary>
    ///     Counts words in the given text based on the available white-spaces.
    /// </summary>
    public static int CountWordsInText(this ReadOnlySpan<char> text)
    {
        if (text.Length == 0)
        {
            return 0;
        }

        var wordCount = 0;
        var index = 0;

        while (index < text.Length && char.IsWhiteSpace(text[index]))
        {
            index++;
        }

        while (index < text.Length)
        {
            while (index < text.Length && !char.IsWhiteSpace(text[index]))
            {
                index++;
            }

            wordCount++;

            while (index < text.Length && char.IsWhiteSpace(text[index]))
            {
                index++;
            }
        }

        return wordCount;
    }

    /// <summary>
    ///     Returns a new string in which the occurrence of a specified string in the current instance is replaced with another
    ///     specified string
    /// </summary>
    public static string? ReplaceAt([NotNullIfNotNull(nameof(input))] this string? input,
        int index,
        int length,
        string? replacement)
    {
        if (length == 0 || input is null || replacement is null)
        {
            return input;
        }

        return string.Create(input.Length - length + replacement.Length, (input, index, length, replacement),
            (span, state) =>
            {
                state.input.AsSpan()[..state.index].CopyTo(span);

                state.replacement.AsSpan().CopyTo(span[state.index..]);

                state.input.AsSpan()[(state.index + state.length)..]
                    .CopyTo(span[(state.index + state.replacement.Length)..]);
            });
    }

    /// <summary>
    ///     Converts the given object to an InvariantCulture string.
    /// </summary>
    public static string? ToInvariantString([NotNullIfNotNull(nameof(value))] this object? value,
        string defaultValue = "")
        => Convert.ToString(value ?? defaultValue, CultureInfo.InvariantCulture);

#if !NET_6 && !NET_7
    /// <summary>
    ///     Creates a string populated with characters chosen at random from choices.
    /// </summary>
    /// <param name="stringLength">The length of string to return.</param>
    /// <param name="allowedChars">
    ///     The characters to use to populate the string.
    ///     Its default value is <![CDATA[ "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()_-+=" ]]>
    /// </param>
    /// <returns>A string populated with random choices.</returns>
    public static string RandomString(this int stringLength,
        string allowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()_-+=")
        => RandomNumberGenerator.GetString(allowedChars, stringLength);

    /// <summary>
    ///     Creates a string filled with cryptographically random hexadecimal characters.
    /// </summary>
    /// <param name="stringLength">The length of string to create.</param>
    /// <param name="lowercase">
    ///     true if the hexadecimal characters should be lowercase; false if they should be uppercase. The
    ///     default is false.
    /// </param>
    /// <returns>A string populated with random hexadecimal characters.</returns>
    public static string RandomHexString(this int stringLength, bool lowercase = false)
        => RandomNumberGenerator.GetHexString(stringLength, lowercase);
#endif
}
