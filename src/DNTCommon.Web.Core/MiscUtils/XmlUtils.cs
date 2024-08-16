using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DNTCommon.Web.Core;

/// <summary>
///     XML utils
/// </summary>
public static class XmlUtils
{
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
}