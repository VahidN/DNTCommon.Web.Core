using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace DNTCommon.Web.Core;

/// <summary>
///     Represents a .EPUB document
/// </summary>
public sealed class EPubDocument : IDisposable
{
    public static readonly XNamespace DcNS = "https://purl.org/dc/elements/1.1/";
    public static readonly XNamespace OpfNS = "https://www.idpf.org/2007/opf";
    private readonly EPubContainer _ePubContainer = new();
    private readonly EPubGuide _ePubGuide = new();
    private readonly EPubManifest _ePubManifest = new();
    private readonly EPubMetadata _ePubMetadata = new();
    private readonly EPubNcx _ePubNcx = new();
    private readonly EPubSpine _ePubSpine = new();
    private readonly Dictionary<string, int> _ids = [];

    private readonly string? _tempDirPath;
    private readonly ZipArchive _zipArchive;
    private bool _disposed;

    /// <summary>
    ///     Creates new instance of .EPUB document
    /// </summary>
    public EPubDocument(string ebookFilePath, string? tempDirPath = null)
    {
        _tempDirPath = tempDirPath;

        if (!_tempDirPath.IsEmpty())
        {
            _tempDirPath.CheckDirExists();
        }

        ebookFilePath.TryDeleteFile();

        _zipArchive = ZipFile.Open(ebookFilePath, ZipArchiveMode.Create, Encoding.UTF8);

        SetupToc();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            _zipArchive.Dispose();
        }
        finally
        {
            _disposed = true;
        }
    }

    /// <summary>
    ///     Add author of the document
    /// </summary>
    /// <param name="author">Human-readable full name</param>
    public void AddAuthor(string author)
    {
        _ePubMetadata.AddAuthor(author);
        _ePubNcx.AddAuthor(author);
    }

    /// <summary>
    ///     Add book identifier
    /// </summary>
    /// <param name="id">A string or number used to uniquely identify the resource</param>
    public void AddBookIdentifier(string id) => AddBookIdentifier(id, string.Empty);

    /// <summary>
    ///     Add book identifier
    /// </summary>
    /// <param name="id">A string or number used to uniquely identify the resource</param>
    /// <param name="scheme">System or authority that generated or assigned the id parameter, for example "ISBN" or "DOI." </param>
    public void AddBookIdentifier(string id, string scheme)
        => _ePubMetadata.AddBookIdentifier(GetNextID(kind: "id"), id, scheme);

    /// <summary>
    ///     Add generic file to document with specified content
    /// </summary>
    /// <param name="epubPath">path in EPUB</param>
    /// <param name="content">file content</param>
    /// <param name="mediaType">MIME media-type, e.g. "application/octet-stream"</param>
    /// <returns>identifier of added file</returns>
    public string AddData(string epubPath, byte[] content, string mediaType)
    {
        WriteFile(epubPath, content);

        return AddEntry(epubPath, mediaType);
    }

    /// <summary>
    ///     Add a DCItem to epub document
    /// </summary>
    public void AddDCItem(string name, string value) => _ePubMetadata.AddDCItem(name, value);

    /// <summary>
    ///     Add description of document's content
    /// </summary>
    /// <param name="description">Document description</param>
    public void AddDescription(string description) => _ePubMetadata.AddDescription(description);

    /// <summary>
    ///     Add generic file to document's contents
    /// </summary>
    /// <param name="path">source file path</param>
    /// <param name="epubPath">path in EPUB</param>
    /// <param name="mediaType">MIME media-type, e.g. "application/octet-stream"</param>
    /// <returns>id of newly added file</returns>
    public string AddFile(string path, string epubPath, string mediaType)
    {
        CopyFile(path, epubPath);

        return AddEntry(epubPath, mediaType);
    }

    /// <summary>
    ///     Add media type or dimensions of the resource. Best practice is to use a value from a controlled vocabulary (e.g.
    ///     MIME media types).
    /// </summary>
    /// <param name="format">document format</param>
    public void AddFormat(string format) => _ePubMetadata.AddFormat(format);

    // Data versions of AddNNN functions
    /// <summary>
    ///     Add image file to document with specified content. Image type
    ///     is detected by filename's extension
    /// </summary>
    /// <param name="epubPath">path in EPUB</param>
    /// <param name="content">file content</param>
    /// <returns>id of newly added file</returns>
    public string AddImageData(string epubPath, byte[] content)
    {
        ArgumentNullException.ThrowIfNull(epubPath);

        WriteFile(epubPath, content);

        return AddImageEntry(epubPath);
    }

    /// <summary>
    ///     Add image to document's contents
    /// </summary>
    /// <param name="path">Path to source image file</param>
    /// <param name="epubPath">Path to image file in EPUB</param>
    /// <returns>id of newly created element</returns>
    public string AddImageFile(string path, string epubPath)
    {
        ArgumentNullException.ThrowIfNull(epubPath);

        CopyFile(path, epubPath);

        return AddImageEntry(epubPath);
    }

    /// <summary>
    ///     Add a language of the intellectual content of the Publication.
    /// </summary>
    /// <param name="lang">RFC3066-complient two-letter language code e.g. "en", "es", "it"</param>
    public void AddLanguage(string lang) => _ePubMetadata.AddLanguage(lang);

    /// <summary>
    ///     Add generic metadata
    /// </summary>
    /// <param name="name">meta element name</param>
    /// <param name="value">meta element value</param>
    public void AddMetaItem(string name, string value) => _ePubMetadata.AddItem(name, value);

    /// <summary>
    ///     Add navigation point to top-level Table of Contents.
    /// </summary>
    /// <param name="label">Text of TOC entry</param>
    /// <param name="content">Link to TOC entry</param>
    /// <param name="playOrder">play order counter</param>
    /// <returns>newly created NavPoint </returns>
    public EPubNavPoint AddNavPoint(string label, string content, int playOrder)
        => _ePubNcx.AddNavPoint(label, GetNextID(kind: "navid"), content, playOrder);

    /// <summary>
    ///     Add reference to guide
    /// </summary>
    /// <param name="href">href of guide reference</param>
    /// <param name="type">type of guide reference</param>
    public void AddReference(string href, string type) => _ePubGuide.AddReference(href, type);

    /// <summary>
    ///     Add reference to guide
    /// </summary>
    /// <param name="href">href of guide reference</param>
    /// <param name="type">type of guide reference</param>
    /// <param name="title">title of guide reference</param>
    public void AddReference(string href, string type, string title) => _ePubGuide.AddReference(href, type, title);

    /// <summary>
    ///     Add an identifier of an auxiliary resource and its relationship to the publication.
    /// </summary>
    /// <param name="relation">document relation</param>
    public void AddRelation(string relation) => _ePubMetadata.AddRelation(relation);

    /// <summary>
    ///     Add a statement about rights, or a reference to one.
    /// </summary>
    /// <param name="rights">A statement about rights, or a reference to one</param>
    public void AddRights(string rights) => _ePubMetadata.AddRights(rights);

    /// <summary>
    ///     Add CSS file to document with specified content.
    /// </summary>
    /// <param name="epubPath">path in EPUB</param>
    /// <param name="content">file content</param>
    /// <returns>id of newly added file</returns>
    public string AddStylesheetData(string epubPath, string content)
    {
        WriteFile(epubPath, content);

        return AddStylesheetEntry(epubPath);
    }

    /// <summary>
    ///     Add CSS file to document's contents
    /// </summary>
    /// <param name="path">path to source CSS file</param>
    /// <param name="epubPath">path to destination file in EPUB</param>
    /// <returns>id of newly created element</returns>
    public string AddStylesheetFile(string path, string epubPath)
    {
        CopyFile(path, epubPath);

        return AddStylesheetEntry(epubPath);
    }

    /// <summary>
    ///     Add document subject: phrase or list of keywords
    /// </summary>
    /// <param name="subj">Document's subject</param>
    public void AddSubject(string subj) => _ePubMetadata.AddSubject(subj);

    /// <summary>
    ///     Add title to epub document
    /// </summary>
    /// <param name="title">document's title</param>
    public void AddTitle(string title)
    {
        _ePubMetadata.AddTitle(title);
        _ePubNcx.AddTitle(title);
    }

    /// <summary>
    ///     Add document translator
    /// </summary>
    /// <param name="name">Human-readable full name</param>
    public void AddTranslator(string name) => _ePubMetadata.AddTranslator(name);

    /// <summary>
    ///     Add terms describing general categories, functions, genres, or aggregation levels for content.
    ///     The advised best practice is to select a value from a controlled vocabulary.
    /// </summary>
    /// <param name="type">document type</param>
    public void AddType(string type) => _ePubMetadata.AddType(type);

    /// <summary>
    ///     Add primary or auxiliary XHTML file to document with specified content.
    /// </summary>
    /// <param name="epubPath">path in EPUB</param>
    /// <param name="content">file content</param>
    /// <param name="primary">true if file is primary, false if auxiliary</param>
    /// <returns>identifier of added file</returns>
    public string AddXhtmlData(string epubPath, string content, bool primary)
    {
        WriteFile(epubPath, content);

        return AddXhtmlEntry(epubPath, primary);
    }

    /// <summary>
    ///     Add primary  XHTML file to document with specified content.
    /// </summary>
    /// <param name="epubPath">path in EPUB</param>
    /// <param name="content">file contents</param>
    /// <returns>identifier of added file</returns>
    public string AddXhtmlData(string epubPath, string content) => AddXhtmlData(epubPath, content, primary: true);

    /// <summary>
    ///     Add primary or auxiliary (like notes) XHTML file to document's content
    /// </summary>
    /// <param name="path">path to source file</param>
    /// <param name="epubPath">path in epub</param>
    /// <param name="primary">true for primary document, false for auxiliary</param>
    /// <returns>id of newly created element</returns>
    public string AddXhtmlFile(string path, string epubPath, bool primary = true)
    {
        CopyFile(path, epubPath);

        return AddXhtmlEntry(epubPath, primary);
    }

    /// <summary>
    ///     Generate document and save to specified file path
    /// </summary>
    public void Generate()
    {
        WriteMimetype();
        WriteOpf(opfFilePath: "content.opf");
        WriteNcx(ncxFilePath: "toc.ncx");
        WriteContainer();
        WriteAppleiBooksDisplayOptions();
    }

    private string AddEntry(string path, string type)
    {
        var id = GetNextID(kind: "id");
        _ePubManifest.AddItem(id, path, type);

        return id;
    }

    private string AddImageEntry(string path)
    {
        var id = GetNextID(kind: "img");
        var contentType = path.GetMimeType();
        _ePubManifest.AddItem(id, path, contentType);

        return id;
    }

    private string AddStylesheetEntry(string path)
    {
        var id = GetNextID(kind: "stylesheet");
        _ePubManifest.AddItem(id, path, type: "text/css");

        return id;
    }

    private string AddXhtmlEntry(string path, bool linear)
    {
        var id = GetNextID(kind: "html");
        _ePubManifest.AddItem(id, path, type: "application/xhtml+xml");
        _ePubSpine.AddItemRef(id, linear);

        return id;
    }

    private void CopyFile(string path, string epubPath)
    {
        var entryName = $"OPF/{epubPath}";
        var fileInfo = new FileInfo(path);
        _zipArchive.AddToZipArchive(entryName, fileInfo);
        WriteToTempFile(entryName, File.ReadAllBytes(fileInfo.FullName));
    }

    private string GetNextID(string kind)
    {
        string id;

        if (_ids.TryGetValue(kind, out var value))
        {
            _ids[kind] = ++value;
            id = string.Create(CultureInfo.InvariantCulture, $"{kind}{value}");
        }
        else
        {
            id = kind + "1";
            _ids[kind] = 1;
        }

        return id;
    }

    private void SetupToc()
    {
        _ePubManifest.AddItem(id: "ncx", href: "toc.ncx", type: "application/x-dtbncx+xml");
        _ePubSpine.SetToc(toc: "ncx");
        _ePubContainer.AddRootFile(file: "OPF/content.opf", mediaType: "application/oebps-package+xml");

        var uuid = "urn:uuid:" + Guid.NewGuid();
        _ePubNcx.SetUid(uuid);
        _ePubMetadata.AddBookIdentifier(id: "BookId", uuid);
    }

    private void WriteAppleiBooksDisplayOptions()
    {
        using var memoryStream = new MemoryStream();

        new XElement(name: "display_options",
                new XElement(name: "platform", new XAttribute(name: "name", value: "*"),
                    new XElement(name: "option", new XAttribute(name: "name", value: "specified-fonts"), true)))
            .Save(memoryStream);

        const string EntryName = "META-INF/com.apple.ibooks.display-options.xml";
        var data = memoryStream.ToArray();
        _zipArchive.AddToZipArchive(EntryName, data);
        WriteToTempFile(EntryName, data);
    }

    private void WriteContainer()
    {
        using var memoryStream = new MemoryStream();

        _ePubContainer.ToElement().Save(memoryStream);
        const string EntryName = "META-INF/container.xml";
        var data = memoryStream.ToArray();
        _zipArchive.AddToZipArchive(EntryName, data);
        WriteToTempFile(EntryName, data);
    }

    private void WriteFile(string epubPath, byte[] content)
    {
        var entryName = $"OPF/{epubPath}";
        _zipArchive.AddToZipArchive(entryName, content);
        WriteToTempFile(entryName, content);
    }

    private void WriteFile(string epubPath, string content)
    {
        var entryName = $"OPF/{epubPath}";
        _zipArchive.AddToZipArchive(entryName, content);
        WriteToTempFile(entryName, content);
    }

    private void WriteMimetype()
    {
        const string EntryName = "mimetype";
        const string Data = "application/epub+zip";
        _zipArchive.AddToZipArchive(EntryName, Data);
        WriteToTempFile(EntryName, Data);
    }

    private void WriteNcx(string ncxFilePath)
    {
        using var memoryStream = new MemoryStream();

        _ePubNcx.ToXmlDocument().Save(memoryStream);

        var entryName = $"OPF/{ncxFilePath}";
        var data = memoryStream.ToArray();
        _zipArchive.AddToZipArchive(entryName, data);
        WriteToTempFile(entryName, data);
    }

    private void WriteOpf(string opfFilePath)
    {
        using var memoryStream = new MemoryStream();

        var packageElement = new XElement(OpfNS + "package", new XAttribute(name: "version", value: "2.0"),
            new XAttribute(name: "unique-identifier", value: "BookId"));

        packageElement.Add(_ePubMetadata.ToElement());
        packageElement.Add(_ePubManifest.ToElement());
        packageElement.Add(_ePubSpine.ToElement());
        packageElement.Add(_ePubGuide.ToElement());
        packageElement.Save(memoryStream);

        var entryName = $"OPF/{opfFilePath}";
        var data = memoryStream.ToArray();
        _zipArchive.AddToZipArchive(entryName, data);
        WriteToTempFile(entryName, data);
    }

    private void WriteToTempFile(string entryName, byte[] data)
    {
        if (!_tempDirPath.IsEmpty())
        {
            var filePath = _tempDirPath.SafePathCombine(entryName);
            filePath.CheckFileDirExists();
            File.WriteAllBytes(filePath, data);
        }
    }

    private void WriteToTempFile(string entryName, string data)
    {
        if (!_tempDirPath.IsEmpty())
        {
            var filePath = _tempDirPath.SafePathCombine(entryName);
            filePath.CheckFileDirExists();
            File.WriteAllText(filePath, data);
        }
    }
}
