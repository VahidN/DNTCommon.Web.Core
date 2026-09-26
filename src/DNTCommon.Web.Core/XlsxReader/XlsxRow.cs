namespace DNTCommon.Web.Core;

public sealed class XlsxRow(int rowNumber)
{
    private readonly Dictionary<int, XlsxCell> _cells = new();

    public int RowNumber { get; } = rowNumber;

    public void Add(XlsxCell cell)
    {
        ArgumentNullException.ThrowIfNull(cell);
        _cells[cell.Column] = cell;
    }

    public XlsxCell? GetCell(int column) => _cells.TryGetValue(column, out var cell) ? cell : null;

    public string GetValue(int column) => GetCell(column)?.Value?.Trim() ?? string.Empty;
}
