namespace DNTCommon.Web.Core;

/// <summary>
///     NameValidator Extensions
/// </summary>
public static class NameValidatorExtensions
{
    /// <summary>
    ///     Determines whether the input text has consecutive chars or not
    /// </summary>
    /// <param name="inputText"></param>
    /// <param name="sequenceLength"></param>
    /// <returns></returns>
    public static bool HasConsecutiveChars(this string? inputText, int sequenceLength = 3)
    {
        if (string.IsNullOrWhiteSpace(inputText))
        {
            return false;
        }

        var charEnumerator = StringInfo.GetTextElementEnumerator(inputText);
        var currentElement = string.Empty;
        var count = 1;

        while (charEnumerator.MoveNext())
        {
            if (string.Equals(currentElement, charEnumerator.GetTextElement(), StringComparison.OrdinalIgnoreCase))
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
}