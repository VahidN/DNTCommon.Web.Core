using System.Text.RegularExpressions;

namespace DNTCommon.Web.Core;

/// <summary>
///     Regex Utils
/// </summary>
public static class RegexUtils
{
    /// <summary>
    ///     Indicates whether the regular expression specified in the Regex constructor finds a match in a specified input
    ///     string.
    /// </summary>
    public static bool IsMatch(this string? input, Regex? regex) => input is not null && regex?.IsMatch(input) == true;

    /// <summary>
    ///     Searches the specified input string for all occurrences of a regular expression.
    /// </summary>
    public static IEnumerable<Match> FindMatches(this string? input, Regex? regex)
    {
        if (input is null || regex is null)
        {
            yield break;
        }

        foreach (Match match in regex.Matches(input))
        {
            if (match.Success)
            {
                yield return match;
            }
        }
    }

    /// <summary>
    ///     Searches the specified input string for all occurrences of a regular expression.
    /// </summary>
    public static IEnumerable<Match> FindUniqueMatches(this string? input, Regex? regex)
    {
        var matches = new HashSet<string>(StringComparer.Ordinal);

        foreach (var match in input.FindMatches(regex))
        {
            if (matches.Add(match.Value))
            {
                yield return match;
            }
        }
    }

    /// <summary>
    ///     Checks if a regular expression parsing error occurred.
    /// </summary>
    public static bool IsValidRegexPattern(
#if !NET_6
        [StringSyntax(syntax: "Regex")] 
#endif
        this string? regexPattern,
        TimeSpan parserTimeout,
        RegexOptions regexOptions = RegexOptions.None)
    {
        if (regexPattern.IsEmpty())
        {
            return false;
        }

        try
        {
            _ = Regex.Match(input: "", regexPattern, regexOptions, parserTimeout);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
