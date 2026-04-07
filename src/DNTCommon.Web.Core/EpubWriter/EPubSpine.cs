using System.Xml.Linq;

namespace DNTCommon.Web.Core;

public class EPubSpine
{
    private readonly List<ItemRef> _itemRefs = [];
    private string? _toc;

    public void AddItemRef(string id, bool linear)
    {
        ItemRef itemRef = new(id, linear);
        _itemRefs.Add(itemRef);
    }

    public void SetToc(string toc) => _toc = toc;

    public XElement ToElement()
    {
        var element = new XElement(EPubDocument.OpfNS + "spine");

        if (!string.IsNullOrEmpty(_toc))
        {
            element.Add(new XAttribute(name: "toc", _toc));
        }

        foreach (var itemRef in _itemRefs)
        {
            var item = new XElement(EPubDocument.OpfNS + "itemref", new XAttribute(name: "idref", itemRef.Id));

            if (!itemRef.Linear)
            {
                item.SetAttributeValue(name: "linear", value: "no");
            }

            element.Add(item);
        }

        return element;
    }

    private sealed record ItemRef(string Id, bool Linear);
}
