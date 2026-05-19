#if NET_10
namespace DNTCommon.Web.Core;

/// <summary>
///     .NET natural string comparison
/// </summary>
public static class NaturalStringComparer
{
    extension(StringComparer)
    {
        /// <summary>
        ///     The option that specifies that string comparisons sort sequences of digits (Unicode general category "Nd") based on
        ///     their numeric value.
        /// </summary>
        public static StringComparer InvariantCultureNumericOrdering
            => StringComparer.Create(CultureInfo.InvariantCulture, CompareOptions.NumericOrdering);

        /// <summary>
        ///     The option that specifies that string comparisons sort sequences of digits (Unicode general category "Nd") based on
        ///     their numeric value. Also indicates that the string comparison ignores case differences.
        /// </summary>
        public static StringComparer InvariantCultureNumericOrderingIgnoreCase
            => StringComparer.Create(CultureInfo.InvariantCulture,
                CompareOptions.IgnoreCase | CompareOptions.NumericOrdering);

        /// <summary>
        ///     The option that specifies that string comparisons sort sequences of digits (Unicode general category "Nd") based on
        ///     their numeric value. Also indicates that the string comparison ignores nonspacing combining characters, such as
        ///     diacritics. Nonspacing characters modify base characters without occupying their own space. The Unicode Standard
        ///     defines combining characters as characters that are combined with base characters to produce a new character.
        /// </summary>
        public static StringComparer InvariantCultureNumericOrderingIgnoreNonSpace => StringComparer.Create(
            CultureInfo.InvariantCulture, CompareOptions.IgnoreNonSpace | CompareOptions.NumericOrdering);

        /// <summary>
        ///     The option that specifies that string comparisons sort sequences of digits (Unicode general category "Nd") based on
        ///     their numeric value. Also indicates that the string comparison ignores symbols, including whitespace, punctuation,
        ///     currency symbols, the percent sign, mathematical symbols, the ampersand, and similar characters.
        /// </summary>
        public static StringComparer InvariantCultureNumericOrderingIgnoreSymbols => StringComparer.Create(
            CultureInfo.InvariantCulture, CompareOptions.IgnoreSymbols | CompareOptions.NumericOrdering);

        /// <summary>
        ///     The option that specifies that string comparisons sort sequences of digits (Unicode general category "Nd") based on
        ///     their numeric value. Also indicates that the string comparison ignores character width. For example, full-width and
        ///     half-width forms of Japanese katakana characters are considered equal with this option.
        /// </summary>
        public static StringComparer InvariantCultureNumericOrderingIgnoreWidth => StringComparer.Create(
            CultureInfo.InvariantCulture, CompareOptions.IgnoreWidth | CompareOptions.NumericOrdering);
    }
}
#endif
