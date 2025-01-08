using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace DNTCommon.Web.Core;

/// <summary>
///     XML utils
/// </summary>
public static class XmlUtils
{
    private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(value: 1);

    /// <summary>
    ///     Pretty prints the given XML
    /// </summary>
    public static string FormatXml(this string xml, bool throwOnException)
    {
        try
        {
            var doc = XDocument.Parse(xml);

            return doc.ToString();
        }
        catch (Exception)
        {
            if (throwOnException)
            {
                throw;
            }

            return xml;
        }
    }

    /// <summary>
    ///     TextSyndicationContent's input sanitizer
    /// </summary>
    public static string SanitizeXmlString(this string? input)
    {
        if (input is null)
        {
            return "";
        }

        var sb = new StringBuilder(input.Length);
        Span<char> chars = stackalloc char[2];

        foreach (var rune in input.EnumerateRunes())
        {
            if (!rune.TryEncodeToUtf16(chars, out var written))
            {
                continue;
            }

            if (written == 1)
            {
                if (!XmlConvert.IsXmlChar(chars[index: 0]))
                {
                    continue;
                }
            }
            else if (written == 2)
            {
                if (!XmlConvert.IsXmlSurrogatePair(chars[index: 0], chars[index: 1]))
                {
                    continue;
                }
            }
            else
            {
                throw new InvalidOperationException(Invariant($"written = {written}"));
            }

            sb.Append(chars[..written]);
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Does the given input contain XHTML?
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsXHTML(this string? input)
    {
        try
        {
            if (input.IsEmpty())
            {
                return false;
            }

            var x = XElement.Parse($"<wrapper>{input}</wrapper>");

            return !(x.DescendantNodes().Count() == 1 && x.DescendantNodes().First().NodeType == XmlNodeType.Text);
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    ///     Does the given input contain HTML?
    /// </summary>
    public static bool ContainsHtmlTag(this string? text, string tagName)
        => !text.IsEmpty() && Regex.IsMatch(text, $@"<\s*{tagName}\s*\/?>", RegexOptions.IgnoreCase, OneMinute);

    /// <summary>
    ///     Does the given input contain HTML?
    /// </summary>
    public static bool ContainsHtmlTags(this string? text, string tagNames)
    {
        ArgumentNullException.ThrowIfNull(tagNames);

        return !text.IsEmpty() && tagNames.Split(separator: '|').Any(text.ContainsHtmlTag);
    }

    /// <summary>
    ///     Does the given input contain HTML?
    /// </summary>
    public static bool ContainsHtmlTags(this string? text)
        => text.ContainsHtmlTags(
            tagNames:
            "a|abbr|acronym|address|area|b|base|bdo|big|blockquote|body|br|button|caption|cite|code|col|colgroup|dd|del|dfn|div|dl|DOCTYPE|dt|em|fieldset|form|h1|h2|h3|h4|h5|h6|head|html|hr|i|img|input|ins|kbd|label|legend|li|link|map|meta|noscript|object|ol|optgroup|option|p|param|pre|q|samp|script|select|small|span|strong|style|sub|sup|table|tbody|td|textarea|tfoot|th|thead|title|tr|tt|ul|var");
}