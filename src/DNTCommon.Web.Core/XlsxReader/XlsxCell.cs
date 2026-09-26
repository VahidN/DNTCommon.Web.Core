namespace DNTCommon.Web.Core;

public sealed class XlsxCell
{
    public int Row { get; set; }

    public int Column { get; set; }

    public string? Value { get; set; }

    public string? Type { get; set; }

    public string? Style { get; set; }

    public string? Formula { get; set; }
}
