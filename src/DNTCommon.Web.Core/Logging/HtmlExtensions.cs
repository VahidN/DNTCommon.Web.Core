using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Html utils
/// </summary>
public static class HtmlExtensions
{
    /// <summary>
    ///     Creates a simple bootstrap table
    /// </summary>
    /// <param name="caption"></param>
    /// <param name="headers"></param>
    /// <param name="rows"></param>
    /// <returns></returns>
    public static string CreateHtmlTable(string caption,
        IEnumerable<string> headers,
        IEnumerable<IEnumerable<string>> rows)
    {
        ArgumentNullException.ThrowIfNull(headers);
        ArgumentNullException.ThrowIfNull(rows);

        var sb = new StringBuilder();
        sb.AppendLine("<br/>");
        sb.AppendLine("<table class=\"table table-sm caption-top table-striped table-hover\">");
        sb.AppendLine(CultureInfo.InvariantCulture, $"<caption>{caption}</caption>");
        sb.AppendLine("<thead><tr>");

        foreach (var header in headers)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"<th scope=\"col\">{header}</th>");
        }

        sb.AppendLine("</tr></thead>");

        sb.AppendLine("<tbody class=\"table-group-divider\">");

        foreach (var row in rows)
        {
            sb.AppendLine("<tr>");

            foreach (var item in row)
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"<td>{item}</td>");
            }

            sb.AppendLine("</tr>");
        }

        sb.AppendLine("</tbody>");
        sb.AppendLine("</table>");

        return sb.ToString();
    }
}