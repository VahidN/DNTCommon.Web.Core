using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Html utils
/// </summary>
public static class HtmlExtensions
{
    /// <summary>
    ///     Creates a `strong` html tag
    /// </summary>
    public static string MakeItStrong(this string text) => $"<strong>{text}</strong>";

    /// <summary>
    ///     Creates a simple bootstrap table
    /// </summary>
    public static string CreateHtmlTable(string? caption,
        IList<string>? headers,
        IEnumerable<IEnumerable<string>> rows,
        string tableClass = "table table-bordered table-sm caption-top table-striped table-hover w-auto mx-auto",
        string? tableAttributes = null)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var sb = new StringBuilder();
        sb.AppendLine(CultureInfo.InvariantCulture, $"<table {tableAttributes} class='{tableClass}'>");

        if (!string.IsNullOrWhiteSpace(caption))
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"<caption>{caption}</caption>");
        }

        if (headers?.Count > 0)
        {
            sb.AppendLine(value: "<thead><tr>");

            foreach (var header in headers)
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"<th scope=\"col\">{header}</th>");
            }

            sb.AppendLine(value: "</tr></thead>");
        }

        sb.AppendLine(value: "<tbody class=\"table-group-divider\">");

        foreach (var row in rows)
        {
            sb.AppendLine(value: "<tr>");

            foreach (var item in row)
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"<td>{item}</td>");
            }

            sb.AppendLine(value: "</tr>");
        }

        sb.AppendLine(value: "</tbody>");
        sb.AppendLine(value: "</table>");

        return sb.ToString();
    }
}