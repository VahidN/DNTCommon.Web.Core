using System.Xml.Linq;

namespace DNTCommon.Web.Core;

public class EPubMetadata
{
    private readonly List<EPubDcItem> _dcItems = [];
    private readonly List<EPubItem> _items = [];

    public void AddAuthor(string name) => AddCreator(name, role: "aut");

    public void AddBookIdentifier(string id, string uuid) => AddBookIdentifier(id, uuid, string.Empty);

    public void AddBookIdentifier(string id, string uuid, string scheme)
    {
        var dcitem = new EPubDcItem(name: "identifier", uuid);
        dcitem.SetAttribute(attributeName: "id", id);

        if (!string.IsNullOrEmpty(scheme))
        {
            dcitem.SetOpfAttribute(attributeName: "scheme", scheme);
        }

        _dcItems.Add(dcitem);
    }

    public void AddContributor(string name, string role)
    {
        var dcitem = new EPubDcItem(name: "contributor", name);
        dcitem.SetOpfAttribute(attributeName: "role", role);
        _dcItems.Add(dcitem);
    }

    public void AddCreator(string name, string role)
    {
        var dcitem = new EPubDcItem(name: "creator", name);
        dcitem.SetOpfAttribute(attributeName: "role", role);
        _dcItems.Add(dcitem);
    }

    public void AddDCItem(string name, string value) => _dcItems.Add(new EPubDcItem(name, value));

    public void AddDescription(string description) => _dcItems.Add(new EPubDcItem(name: "description", description));

    public void AddFormat(string format) => _dcItems.Add(new EPubDcItem(name: "format", format));

    public void AddItem(string name, string value) => _items.Add(new EPubItem(name, value));

    public void AddLanguage(string lang) => _dcItems.Add(new EPubDcItem(name: "language", lang));

    public void AddRelation(string relation) => _dcItems.Add(new EPubDcItem(name: "relation", relation));

    public void AddRights(string rights) => _dcItems.Add(new EPubDcItem(name: "rights", rights));

    public void AddSubject(string subj) => _dcItems.Add(new EPubDcItem(name: "subject", subj));

    public void AddTitle(string title) => _dcItems.Add(new EPubDcItem(name: "title", title));

    public void AddTranslator(string name) => AddCreator(name, role: "trl");

    public void AddType(string type) => _dcItems.Add(new EPubDcItem(name: "type", type));

    public XElement ToElement()
    {
        XNamespace dc = "https://purl.org/dc/elements/1.1/";
        XNamespace opf = "https://www.idpf.org/2007/opf";

        var element = new XElement(EPubDocument.OpfNS + "metadata", new XAttribute(XNamespace.Xmlns + "dc", dc),
            new XAttribute(XNamespace.Xmlns + "opf", opf));

        foreach (var item in _items)
        {
            element.Add(item.ToElement());
        }

        foreach (var item in _dcItems)
        {
            element.Add(item.ToElement());
        }

        return element;
    }
}
