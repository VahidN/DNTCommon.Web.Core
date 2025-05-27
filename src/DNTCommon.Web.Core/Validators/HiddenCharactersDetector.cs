namespace DNTCommon.Web.Core;

/// <summary>
///     A detector and cleaner for hidden, typographic, and ideographic variation selector (IVS) Unicode characters in text
///     files.
/// </summary>
public static class HiddenCharactersDetector
{
    /// <summary>
    ///     Define common hidden characters often used for watermarking or causing issues
    /// </summary>
    public static readonly IReadOnlyDictionary<char, string> HiddenMarkers = new Dictionary<char, string>
    {
        // Zero Width Characters
        [key: '\u200B'] = "Zero Width Space (U+200B)",
        [key: '\u200C'] = "Zero Width Non-Joiner (U+200C)",
        [key: '\u200D'] = "Zero Width Joiner (U+200D)",
        [key: '\u2060'] = "Word Joiner (U+2060)",
        [key: '\uFEFF'] = "Byte Order Mark (BOM) / Zero Width No-Break Space (U+FEFF)",

        // Common Non-Standard Spaces
        [key: '\u00A0'] = "Non-Breaking Space (U+00A0)",
        [key: '\u202F'] = "Narrow No-Break Space (U+202F)",

        // Other Fixed-Width or Special Spaces
        [key: '\u2000'] = "En Quad (U+2000)",
        [key: '\u2001'] = "Em Quad (U+2001)",
        [key: '\u2002'] = "En Space (U+2002)",
        [key: '\u2003'] = "Em Space (U+2003)",
        [key: '\u2004'] = "Three-Per-Em Space (U+2004)",
        [key: '\u2005'] = "Four-Per-Em Space (U+2005)",
        [key: '\u2006'] = "Six-Per-Em Space (U+2006)",
        [key: '\u2007'] = "Figure Space (U+2007)",
        [key: '\u2008'] = "Punctuation Space (U+2008)",
        [key: '\u2009'] = "Thin Space (U+2009)",
        [key: '\u200A'] = "Hair Space (U+200A)",
        [key: '\u205F'] = "Medium Mathematical Space (U+205F)",
        [key: '\u3000'] = "Ideographic Space (U+3000)",

        // Other Invisible or Control-like Characters
        [key: '\u180E'] = "Mongolian Vowel Separator (U+180E)",
        [key: '\u034F'] = "Combining Grapheme Joiner (U+034F)",
        [key: '\u00AD'] = "Soft Hyphen (U+00AD)",

        // Directional Formatting Characters
        [key: '\u200E'] = "Left-to-Right Mark",
        [key: '\u200F'] = "Right-to-Left Mark",
        [key: '\u202A'] = "Left-to-Right Embedding",
        [key: '\u202B'] = "Right-to-Left Embedding",
        [key: '\u202C'] = "Pop Directional Formatting",
        [key: '\u202D'] = "Left-to-Right Override",
        [key: '\u202E'] = "Right-to-Left Override",
        [key: '\u2061'] = "Function Application",
        [key: '\u2062'] = "Invisible Times",
        [key: '\u2063'] = "Invisible Separator",
        [key: '\u2064'] = "Invisible Plus",
        [key: '\u2066'] = "Left-to-Right Isolate",
        [key: '\u2067'] = "Right-to-Left Isolate",
        [key: '\u2068'] = "First Strong Isolate",
        [key: '\u2069'] = "Pop Directional Isolate",

        // Variation Selectors
        [key: '\uFE00'] = "Variation Selector-1 (U+FE00)",
        [key: '\uFE01'] = "Variation Selector-2 (U+FE01)",
        [key: '\uFE02'] = "Variation Selector-3 (U+FE02)",
        [key: '\uFE03'] = "Variation Selector-4 (U+FE03)",
        [key: '\uFE04'] = "Variation Selector-5 (U+FE04)",
        [key: '\uFE05'] = "Variation Selector-6 (U+FE05)",
        [key: '\uFE06'] = "Variation Selector-7 (U+FE06)",
        [key: '\uFE07'] = "Variation Selector-8 (U+FE07)",
        [key: '\uFE08'] = "Variation Selector-9 (U+FE08)",
        [key: '\uFE09'] = "Variation Selector-10 (U+FE09)",
        [key: '\uFE0A'] = "Variation Selector-11 (U+FE0A)",
        [key: '\uFE0B'] = "Variation Selector-12 (U+FE0B)",
        [key: '\uFE0C'] = "Variation Selector-13 (U+FE0C)",
        [key: '\uFE0D'] = "Variation Selector-14 (U+FE0D)",
        [key: '\uFE0E'] = "Variation Selector-15 (U+FE0E)",
        [key: '\uFE0F'] = "Variation Selector-16 (U+FE0F)",

        // Mongolian Free Variation Selectors
        [key: '\u180B'] = "Mongolian Free Variation Selector One (FVS1, U+180B)",
        [key: '\u180C'] = "Mongolian Free Variation Selector Two (FVS2, U+180C)",
        [key: '\u180D'] = "Mongolian Free Variation Selector Three (FVS3, U+180D)"
    };

    /// <summary>
    ///     Typographic characters whose usage might be of interest
    /// </summary>
    public static readonly IReadOnlyDictionary<char, string> TypographicMarkers = new Dictionary<char, string>
    {
        [key: '\u2010'] = "Hyphen (U+2010)",
        [key: '\u2011'] = "Non-Breaking Hyphen (U+2011)",
        [key: '\u2013'] = "En Dash (U+2013)",
        [key: '\u2014'] = "Em Dash (U+2014)",
        [key: '\u2026'] = "Horizontal Ellipsis (U+2026)",
        [key: '\u2022'] = "Bullet (U+2022)",
        [key: '\u2018'] = "Left Single Quotation Mark (U+2018)",
        [key: '\u2019'] = "Right Single Quotation Mark (U+2019)",
        [key: '\u201C'] = "Left Double Quotation Mark (U+201C)",
        [key: '\u201D'] = "Right Double Quotation Mark (U+201D)"
    };

    /// <summary>
    ///     Characters to replace with their "correct" counterpart.
    /// </summary>
    public static readonly IReadOnlyDictionary<char, char> TypographicReplacements = new Dictionary<char, char>
    {
        [key: '\u2018'] = '\'',
        [key: '\u2019'] = '\'',
        [key: '\u201C'] = '"',
        [key: '\u201D'] = '"',
        [key: '\u2022'] = '*',
        [key: '\u2010'] = '-',
        [key: '\u2011'] = '-',
        [key: '\u2013'] = '-',
        [key: '\u2014'] = '-'
    };

    /// <summary>
    ///     A detector for hidden, typographic, and ideographic variation selector (IVS) Unicode characters
    /// </summary>
    public static bool HasHiddenCharacters(this string? text)
        => !text.IsEmpty() && text.Any(ch => ch.IsHiddenCharacter());

    /// <summary>
    ///     A detector for hidden, typographic, and ideographic variation selector (IVS) Unicode characters
    /// </summary>
    public static bool IsHiddenCharacter(this char? ch)
        => ch.HasValue && (HiddenMarkers.ContainsKey(ch.Value) || TypographicMarkers.ContainsKey(ch.Value));

    /// <summary>
    ///     A detector for hidden, typographic, and ideographic variation selector (IVS) Unicode characters
    /// </summary>
    public static bool IsHiddenCharacter(this char ch)
        => HiddenMarkers.ContainsKey(ch) || TypographicMarkers.ContainsKey(ch);
}
