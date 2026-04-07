using System.Xml.Linq;

namespace DNTCommon.Web.Core;

public class EPubManifest
{
    private readonly XElement _element = new(EPubDocument.OpfNS + "manifest");

    public void AddItem(string id, string href, string type)
    {
        var item = new XElement(EPubDocument.OpfNS + "item");
        item.SetAttributeValue(name: "id", id);
        item.SetAttributeValue(name: "href", href);
        item.SetAttributeValue(name: "media-type", type);
        _element.Add(item);
    }

    public XElement ToElement() => _element;
}
