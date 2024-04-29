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
    /// <param name="xml"></param>
    /// <returns></returns>
    public static string FormatXml(this string xml)
    {
        try
        {
            var doc = XDocument.Parse(xml);

            return doc.ToString();
        }
        catch (Exception)
        {
            return xml;
        }
    }
}