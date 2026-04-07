using System.Xml.Linq;

namespace DNTCommon.Web.Core;

public class EPubItem(string name, string value)
{
    public XElement ToElement()
    {
        var element = new XElement(EPubDocument.OpfNS + "meta");
        element.SetAttributeValue(name: "name", name);
        element.SetAttributeValue(name: "content", value);

        return element;
    }
}
