namespace DNTCommon.Web.Core;

/// <summary>
///     Char Utils
/// </summary>
public static class CharUtils
{
    /// <summary>
    ///     Extracts a set of chars from the given text based on the provided predicate
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? ExtractCharsWhere(this string? text, Predicate<char> predicate)
        => text is null ? null : string.Concat(text.Where(c => predicate(c)));

    /// <summary>
    ///     Removes a set of chars from the given text based on the provided predicate
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? RemoveCharsWhere(this string? text, Predicate<char> predicate)
        => text is null ? null : string.Concat(text.Where(c => !predicate(c)));

    /// <summary>
    ///     Checks if all chars of the given text satisfy the provided predicate
    /// </summary>
    public static bool ContainsOnlyWhere([NotNullWhen(returnValue: true)] this string? text, Predicate<char> predicate)
        => text?.All(c => !predicate(c)) == true;

    /// <summary>
    ///     Checks if any char of the given text satisfies the provided predicate
    /// </summary>
    public static bool ContainsAnyWhere([NotNullWhen(returnValue: true)] this string? text, Predicate<char> predicate)
        => text?.Any(c => !predicate(c)) == true;

    /// <summary>
    ///     Extracts Unicode letters from the given text.
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? ExtractLetters(this string? text) => text.ExtractCharsWhere(char.IsLetter);

    /// <summary>
    ///     Indicates whether the specified text is consists of letters or decimal digits.
    /// </summary>
    public static bool ContainsOnlyLettersOrDigits([NotNullWhen(returnValue: true)] this string? text)
        => text.ContainsOnlyWhere(char.IsLetterOrDigit);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string is categorized as a control
    ///     character.
    /// </summary>
    public static bool IsControl(this string? text, int index) => text is not null && char.IsControl(text, index);

    /// <summary>
    ///     Indicates whether the specified text contains only decimal digits.
    /// </summary>
    public static bool ContainsOnlyDigits(this string? text) => text.ContainsOnlyWhere(char.IsDigit);

    /// <summary>
    ///     Indicates whether the specified text contains any decimal digits.
    /// </summary>
    public static bool ContainsDigits(this string? text) => text.ContainsAnyWhere(char.IsDigit);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string is categorized as a decimal digit.
    /// </summary>
    public static bool IsDigit(this string? text, int index) => text is not null && char.IsDigit(text, index);

    /// <summary>
    ///     Indicates whether the Char object at the specified position in a string is a high surrogate.
    /// </summary>
    public static bool IsHighSurrogate(this string? text, int index)
        => text is not null && char.IsHighSurrogate(text, index);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string is categorized as a Unicode letter.
    /// </summary>
    public static bool IsLetter(this string? text, int index) => text is not null && char.IsLetter(text, index);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string is categorized as a letter or a
    ///     decimal digit.
    /// </summary>
    public static bool IsLetterOrDigit(this string? text, int index)
        => text is not null && char.IsLetterOrDigit(text, index);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string is categorized as a lowercase
    ///     letter.
    /// </summary>
    public static bool IsLower(this string? text, int index) => text is not null && char.IsLower(text, index);

    /// <summary>
    ///     Indicates whether the Char object at the specified position in a string is a low surrogate.
    /// </summary>
    public static bool IsLowSurrogate(this string? text, int index)
        => text is not null && char.IsLowSurrogate(text, index);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string is categorized as a number.
    /// </summary>
    public static bool IsNumber(this string? text, int index) => text is not null && char.IsNumber(text, index);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string is categorized as a punctuation
    ///     mark.
    /// </summary>
    public static bool IsPunctuation(this string? text, int index)
        => text is not null && char.IsPunctuation(text, index);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string is categorized as a separator
    ///     character.
    /// </summary>
    public static bool IsSeparator(this string? text, int index) => text is not null && char.IsSeparator(text, index);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string has a surrogate code unit.
    /// </summary>
    public static bool IsSurrogate(this string? text, int index) => text is not null && char.IsSurrogate(text, index);

    /// <summary>
    ///     Indicates whether two adjacent Char objects at a specified position in a string form a surrogate pair.
    /// </summary>
    public static bool IsSurrogatePair(this string? text, int index)
        => text is not null && char.IsSurrogatePair(text, index);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string is categorized as a symbol
    ///     character.
    /// </summary>
    public static bool IsSymbol(this string? text, int index) => text is not null && char.IsSymbol(text, index);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string is categorized as an uppercase
    ///     letter.
    /// </summary>
    public static bool IsUpper(this string? text, int index) => text is not null && char.IsUpper(text, index);

    /// <summary>
    ///     Indicates whether the character at the specified position in a specified string is categorized as white space.
    /// </summary>
    public static bool IsWhiteSpace(this string? text, int index) => text is not null && char.IsWhiteSpace(text, index);

    /// <summary>
    ///     Extracts a set of chars from the given text based on the provided predicate (char.IsLetterOrDigit).
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? ExtractLettersOrDigits(this string? text) => text.ExtractCharsWhere(char.IsLetterOrDigit);

    /// <summary>
    ///     Removes a set of chars from the given text based on the provided predicate (char.IsControl).
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? RemoveControlCharacters(this string? text) => text.RemoveCharsWhere(char.IsControl);

    /// <summary>
    ///     Removes a set of chars from the given text based on the provided predicate (char.IsLetter).
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? RemoveLetters(this string? text) => text.RemoveCharsWhere(char.IsLetter);

    /// <summary>
    ///     Removes a set of chars from the given text based on the provided predicate (char.IsDigit).
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? RemoveDigits(this string? text) => text.RemoveCharsWhere(char.IsDigit);

    /// <summary>
    ///     Removes a set of chars from the given text based on the provided predicate (char.IsNumber).
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? RemoveNumbers(this string? text) => text.RemoveCharsWhere(char.IsNumber);

    /// <summary>
    ///     Removes a set of chars from the given text based on the provided predicate (char.IsWhiteSpace).
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? RemoveWhiteSpaces(this string? text) => text.RemoveCharsWhere(char.IsWhiteSpace);
}
