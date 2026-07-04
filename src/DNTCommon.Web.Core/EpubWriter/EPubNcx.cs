using System.Xml.Linq;

namespace DNTCommon.Web.Core;

public class EPubNcx
{
    public static readonly XNamespace NcxNS = "https://www.daisy.org/z3986/2005/ncx/";
    private readonly List<string> _authors = [];
    private readonly List<EPubNavPoint> _navPoints = [];
    private string _title = string.Empty;
    private string? _uid;

    public void SetUid(string uid) => _uid = uid;

    public void AddAuthor(string author) => _authors.Add(author);

    public void AddTitle(string title) => _title += " " + title;

    public void SetTitle(string title) => _title = title;

    public XDocument ToXmlDocument()
    {
        var doc = new XDocument(new XDocumentType(name: "ncx", publicId: "-//NISO//DTD ncx 2005-1//EN",
            systemId: "https://www.daisy.org/z3986/2005/ncx-2005-1.dtd", internalSubset: null));

        var ncx = new XElement(NcxNS + "ncx");
        ncx.Add(CreateHeadElement());

        // create doc data
        ncx.Add(new XElement(NcxNS + "docTitle", new XElement(NcxNS + "text", _title)));

        foreach (var author in _authors)
        {
            ncx.Add(new XElement(NcxNS + "docAuthor", new XElement(NcxNS + "text", author)));
        }

        var navMap = new XElement(NcxNS + "navMap");

        foreach (var navPoint in _navPoints)
        {
            navMap.Add(navPoint.ToElement());
        }

        ncx.Add(navMap);
        doc.Add(ncx);

        FixPlayOrders(doc);

        return doc;
    }

    private static void FixPlayOrders(XDocument doc)
    {
        var elementsWithPlayOrder = doc.Descendants()
            .Where(xElement => xElement.Attributes().Any(xAttribute => xAttribute.Name == "playOrder"));

        var playOrderItem = 1;

        foreach (var elementWithPlayOrder in elementsWithPlayOrder)
        {
            var playOrder = elementWithPlayOrder.Attribute(name: "playOrder");

            playOrder?.Value = playOrderItem++.ToString(CultureInfo.InvariantCulture);
        }
    }

    public EPubNavPoint AddNavPoint(string label, string id, string content, int playOrder)
    {
        var navPoint = new EPubNavPoint(label, id, content, playOrder);
        _navPoints.Add(navPoint);

        return navPoint;
    }

    private XElement CreateHeadElement()
    {
        var head = new XElement(NcxNS + "head");

        if (!string.IsNullOrWhiteSpace(_uid))
        {
            head.Add(new XElement(NcxNS + "meta", new XAttribute(name: "name", value: "dtb:uid"),
                new XAttribute(name: "content", _uid)));
        }

        head.Add(new XElement(NcxNS + "meta", new XAttribute(name: "name", value: "dtb:depth"),
            new XAttribute(name: "content", value: "1")));

        head.Add(new XElement(NcxNS + "meta", new XAttribute(name: "name", value: "dtb:totalPageCount"),
            new XAttribute(name: "content", value: "0")));

        head.Add(new XElement(NcxNS + "meta", new XAttribute(name: "name", value: "dtb:maxPageNumber"),
            new XAttribute(name: "content", value: "0")));

        return head;
    }
}
