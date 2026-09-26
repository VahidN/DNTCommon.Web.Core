using System.IO.Compression;
using System.Xml.Linq;

namespace DNTCommon.Web.Core;

public sealed class SimpleXlsxReader : IDisposable
{
    private const string MainNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    private const string DocumentRelationshipNamespace =
        "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

    private const string PackageRelationshipNamespace = "http://schemas.openxmlformats.org/package/2006/relationships";

    private static readonly XNamespace MainNs = MainNamespace;

    private static readonly XNamespace DocumentRelNs = DocumentRelationshipNamespace;

    private static readonly XNamespace PackageRelNs = PackageRelationshipNamespace;

    private readonly ZipArchive _archive;

    private readonly Dictionary<int, string> _sharedStrings = new();

    public SimpleXlsxReader(string filePath)
    {
        _archive = ZipFile.OpenRead(filePath);
        LoadSharedStrings();
    }

    public string? SelectedWorksheetName { get; private set; }

    public void Dispose() => _archive.Dispose();

    /// <summary>
    ///     Reads rows of the given Excel file.
    /// </summary>
    /// <param name="worksheetName">If it's not specified, the first non-empty worksheet will be used.</param>
    /// <returns></returns>
    public IEnumerable<XlsxRow> ReadRows(string? worksheetName = null)
    {
        var worksheet = FindFirstNonEmptyWorksheet(worksheetName);

        if (worksheet is null)
        {
            yield break;
        }

        SelectedWorksheetName = worksheet.Name;

        if (worksheet.Path is null)
        {
            yield break;
        }

        var document = LoadXml(worksheet.Path);

        var sheetData = document.Root?.Element(MainNs + "sheetData");

        if (sheetData is null)
        {
            yield break;
        }

        var sequentialRow = 1;

        foreach (var rowElement in sheetData.Elements(MainNs + "row"))
        {
            yield return ReadRow(rowElement, sequentialRow++, _sharedStrings);
        }
    }

    public IEnumerable<string> GetWorksheetNames()
    {
        var workbook = LoadXml(path: "xl/workbook.xml");

        var sheets = workbook.Root?.Element(MainNs + "sheets")?.Elements(MainNs + "sheet");

        if (sheets is null)
        {
            yield break;
        }

        foreach (var sheet in sheets)
        {
            var name = (string?)sheet.Attribute(name: "name");

            if (!string.IsNullOrWhiteSpace(name))
            {
                yield return name;
            }
        }
    }

    private XlsxWorksheetInfo? FindFirstNonEmptyWorksheet(string? worksheetName)
    {
        var workbook = LoadXml(path: "xl/workbook.xml");

        var relationships = LoadWorkbookRelationships();

        var sheets = workbook.Root?.Element(MainNs + "sheets")?.Elements(MainNs + "sheet");

        if (sheets is null)
        {
            return null;
        }

        foreach (var sheet in sheets)
        {
            var name = (string?)sheet.Attribute(name: "name");

            var relationshipId = (string?)sheet.Attribute(DocumentRelNs + "id");

            if (string.IsNullOrWhiteSpace(relationshipId))
            {
                continue;
            }

            if (!relationships.TryGetValue(relationshipId, out var target))
            {
                continue;
            }

            var worksheetPath = NormalizeWorksheetPath(target);

            if (worksheetPath is null)
            {
                continue;
            }

            var document = LoadXml(worksheetPath);

            var sheetData = document.Root?.Element(MainNs + "sheetData");

            if (sheetData is null)
            {
                continue;
            }

            var hasData = sheetData.Elements(MainNs + "row").Any(row => row.Elements(MainNs + "c").Any());

            if (!hasData)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(worksheetName) || (!string.IsNullOrWhiteSpace(worksheetName) &&
                                                             string.Equals(name, worksheetName,
                                                                 StringComparison.Ordinal)))
            {
                return new XlsxWorksheetInfo
                {
                    Name = name,
                    Path = worksheetPath
                };
            }
        }

        return null;
    }

    private Dictionary<string, string?> LoadWorkbookRelationships()
    {
        var document = LoadXml(path: "xl/_rels/workbook.xml.rels");

        return document.Root?.Elements(PackageRelNs + "Relationship")
            .Where(x => (string?)x.Attribute(name: "Id") is not null)
            .ToDictionary(x => (string)x.Attribute(name: "Id")!, x => (string?)x.Attribute(name: "Target"),
                StringComparer.Ordinal) ?? new Dictionary<string, string?>(StringComparer.Ordinal);
    }

    private static string? NormalizeWorksheetPath(string? target)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            return null;
        }

        target = target.Replace(oldChar: '\\', newChar: '/');

        if (target.StartsWith(value: '/'))
        {
            return target.TrimStart(trimChar: '/');
        }

        return target.StartsWith(value: "xl/", StringComparison.Ordinal)
            ? target
            : $"xl/{target.TrimStart(trimChar: '/')}";
    }

    private void LoadSharedStrings()
    {
        var entry = _archive.GetEntry(entryName: "xl/sharedStrings.xml");

        if (entry is null)
        {
            return;
        }

        var document = LoadXml(entry);

        var items = document.Root?.Elements(MainNs + "si").ToList();

        if (items is null)
        {
            return;
        }

        for (var i = 0; i < items.Count; i++)
        {
            var text = string.Concat(items[i].Descendants(MainNs + "t").Select(x => x.Value));

            _sharedStrings[i] = text;
        }
    }

    private static XlsxRow ReadRow(XElement rowElement, int sequentialRow, Dictionary<int, string> sharedStrings)
    {
        var rowNumber = ((string?)rowElement.Attribute(name: "r")).ToInt(sequentialRow);

        var result = new XlsxRow(rowNumber);

        var nextColumn = 1;

        foreach (var cellElement in rowElement.Elements(MainNs + "c"))
        {
            var cell = ReadCell(cellElement, rowNumber, nextColumn, sharedStrings);

            result.Add(cell);

            nextColumn = cell.Column + 1;
        }

        return result;
    }

    private static XlsxCell ReadCell(XElement cellElement,
        int rowNumber,
        int sequentialColumn,
        Dictionary<int, string> sharedStrings)
    {
        var address = (string?)cellElement.Attribute(name: "r");

        var column = !string.IsNullOrWhiteSpace(address) ? GetColumnNumber(address) : sequentialColumn;

        var type = (string?)cellElement.Attribute(name: "t");

        var style = (string?)cellElement.Attribute(name: "s");

        var formula = cellElement.Element(MainNs + "f")?.Value;

        string? value = null;

        if (string.Equals(type, b: "inlineStr", StringComparison.Ordinal))
        {
            value = string.Concat(cellElement.Descendants(MainNs + "t").Select(x => x.Value));
        }
        else if (string.Equals(type, b: "s", StringComparison.Ordinal))
        {
            var v = cellElement.Element(MainNs + "v")?.Value;

            if (int.TryParse(v, out var index) && sharedStrings.TryGetValue(index, out var sharedValue))
            {
                value = sharedValue;
            }
        }
        else if (string.Equals(type, b: "b", StringComparison.Ordinal))
        {
            var v = cellElement.Element(MainNs + "v")?.Value;

            value = string.Equals(v, b: "1", StringComparison.Ordinal) ? "TRUE" : "FALSE";
        }
        else
        {
            value = cellElement.Element(MainNs + "v")?.Value;
        }

        return new XlsxCell
        {
            Row = rowNumber,
            Column = column,
            Value = value,
            Type = type,
            Style = style,
            Formula = formula
        };
    }

    private static int GetColumnNumber(string address)
    {
        var result = 0;

        foreach (var c in address)
        {
            if (!char.IsLetter(c))
            {
                break;
            }

            result = (result * 26) + (char.ToUpperInvariant(c) - 'A') + 1;
        }

        return result;
    }

    private XDocument LoadXml(string path)
    {
        var entry = _archive.GetEntry(path);

        return entry is null ? throw new FileNotFoundException($"XLSX entry not found: {path}") : LoadXml(entry);
    }

    private static XDocument LoadXml(ZipArchiveEntry entry)
    {
        using var stream = entry.Open();

        return XDocument.Load(stream);
    }
}
