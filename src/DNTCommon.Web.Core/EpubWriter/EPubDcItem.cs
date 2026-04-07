using System.Xml.Linq;

namespace DNTCommon.Web.Core;

public class EPubDcItem(string name, string value)
{
    private readonly Dictionary<string, string> _attributes = [];
    private readonly Dictionary<string, string> _opfAttributes = [];

    public void SetAttribute(string attributeName, string attributeValue)
        => _attributes.Add(attributeName, attributeValue);

    public void SetOpfAttribute(string attributeName, string attributeValue)
        => _opfAttributes.Add(attributeName, attributeValue);

    public XElement ToElement()
    {
        var element = new XElement(EPubDocument.DcNS + name, value);

        foreach (var key in _opfAttributes.Keys)
        {
            element.SetAttributeValue(EPubDocument.OpfNS + key, _opfAttributes[key]);
        }

        foreach (var key in _attributes.Keys)
        {
            element.SetAttributeValue(key, _attributes[key]);
        }

        return element;
    }
}
