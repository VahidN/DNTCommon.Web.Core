using System.Xml.Linq;

namespace DNTCommon.Web.Core;

/// <summary>
///     Class for TOC entry. Top-level navPoints should be created by Epub.Document.AddNavPoint method
/// </summary>
public class EPubNavPoint(string label, string id, string content, int playOrder, string @class = "")
{
    private readonly List<EPubNavPoint> _navPoints = [];

    /// <summary>
    ///     Add TOC entry as a direct child of this NavPoint
    /// </summary>
    /// <param name="label">Text of TOC entry</param>
    /// <param name="content">Link to TOC entry</param>
    /// <param name="playOrder">play order counter</param>
    /// <returns>newly created NavPoint </returns>
    public EPubNavPoint AddNavPoint(string label, string content, int playOrder)
    {
        var id1 = string.Create(CultureInfo.InvariantCulture, $"{id}x{_navPoints.Count + 1}");
        var nav = new EPubNavPoint(label, id1, content, playOrder);
        _navPoints.Add(nav);

        return nav;
    }

    public XElement ToElement()
    {
        var element = new XElement(EPubNcx.NcxNS + "navPoint", new XAttribute(name: "id", id),
            new XAttribute(name: "playOrder", playOrder));

        if (!string.IsNullOrEmpty(@class))
        {
            element.Add(new XAttribute(name: "class", @class));
        }

        element.Add(new XElement(EPubNcx.NcxNS + "navLabel", new XElement(EPubNcx.NcxNS + "text", label)));
        element.Add(new XElement(EPubNcx.NcxNS + "content", new XAttribute(name: "src", content)));

        foreach (var navPoint in _navPoints)
        {
            element.Add(navPoint.ToElement());
        }

        return element;
    }
}
