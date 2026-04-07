using System.Xml.Linq;

namespace DNTCommon.Web.Core;

public class EPubContainer
{
    private readonly List<RootFile> _rootFiles = [];

    public void AddRootFile(string file, string mediaType) => _rootFiles.Add(new RootFile(file, mediaType));

    public XElement ToElement()
    {
        XNamespace ns = "urn:oasis:names:tc:opendocument:xmlns:container";
        var element = new XElement(ns + "container", new XAttribute(name: "version", value: "1.0"));
        var filesElement = new XElement(ns + "rootfiles");

        foreach (var rootFile in _rootFiles)
        {
            var fileElement = new XElement(ns + "rootfile", new XAttribute(name: "full-path", rootFile.File),
                new XAttribute(name: "media-type", rootFile.MediaType));

            filesElement.Add(fileElement);
        }

        element.Add(filesElement);

        return element;
    }

    private sealed record RootFile(string File, string MediaType);
}
