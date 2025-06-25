using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Adds invisible chars to the inputText
/// </summary>
public static class TextObfuscator
{
    /// <summary>
    ///     Adds invisible chars to the inputText
    /// </summary>
    public static string? ObfuscateWithHiddenMarkers([NotNullIfNotNull(nameof(inputText))] this string? inputText,
        int injectionLevel = 1)
    {
        if (inputText.IsEmpty())
        {
            return inputText;
        }

        var hiddenMarkersCount = HiddenCharactersDetector.HiddenMarkers.Count;

        var obfuscatedText = new StringBuilder();

        foreach (var chr in inputText)
        {
            obfuscatedText.Append(chr);

            for (var i = 0; i < injectionLevel; i++)
            {
                var randomIndex = RandomNumberGenerator.GetInt32(fromInclusive: 0, hiddenMarkersCount);
                obfuscatedText.Append(HiddenCharactersDetector.HiddenMarkers.Keys.ElementAt(randomIndex));
            }
        }

        return obfuscatedText.ToString();
    }
}
