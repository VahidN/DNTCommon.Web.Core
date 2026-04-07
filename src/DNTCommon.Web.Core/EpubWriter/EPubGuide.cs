using System.Xml.Linq;

namespace DNTCommon.Web.Core;

public class EPubGuide
{
    private readonly XElement _element = new(EPubDocument.OpfNS + "guide");

    public void AddReference(string href, string type, string title = "")
    {
        var itemref = new XElement(EPubDocument.OpfNS + "reference", new XAttribute(name: "href", href),
            new XAttribute(name: "type", type), new XAttribute(name: "title", title));

        if (!string.IsNullOrEmpty(title))
        {
            itemref.SetAttributeValue(name: "title", title);
        }

        _element.Add(itemref);
    }

    public XElement ToElement() => _element;
}
