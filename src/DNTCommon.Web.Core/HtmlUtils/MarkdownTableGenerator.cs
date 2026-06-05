using System.Text;
using System.Text.RegularExpressions;

namespace DNTCommon.Web.Core;

/// <summary>
///     تولید جدول Markdown از روی سرستون‌ها و ردیف‌ها
/// </summary>
public static partial class MarkdownTableGenerator
{
    [GeneratedRegex(pattern: @"([_*\[\]()~`>#+\-=|{}.!])",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 3000)]
    private static partial Regex MarkdownV2();

    /// <summary>
    ///     تولید جدول Markdown از روی سرستون‌ها و ردیف‌ها
    /// </summary>
    /// <param name="headers">لیست نام ستون‌ها</param>
    /// <param name="rows">لیست ردیف‌ها که هر ردیف لیستی از مقادیر است</param>
    /// <returns>رشته‌ی جدول در قالب Markdown</returns>
    public static string GenerateMarkdownTable(this ICollection<string>? headers,
        ICollection<ICollection<string>>? rows)
    {
        var columnCount = headers?.Count ?? 0;

        if (headers is null || columnCount == 0)
        {
            return string.Empty;
        }

        var markdownTable = new StringBuilder();

        // خط سرستون‌ها
        markdownTable.Append(value: '|');
        markdownTable.AppendJoin(separator: '|', headers.Select(EscapeMarkdownTableCell));
        markdownTable.AppendLine(value: "|");

        // خط جداکننده (alignment پیش‌فرض چپ)
        markdownTable.Append(value: '|');
        markdownTable.AppendJoin(separator: '|', headers.Select(_ => "---"));
        markdownTable.AppendLine(value: "|");

        // ردیف‌های داده
        if (rows is null || rows.Count == 0)
        {
            return string.Empty;
        }

        foreach (var row in rows)
        {
            var rowCells = row.Take(columnCount).ToList(); // تعداد ستون‌ها را برابر با هدر می‌کنیم

            if (rowCells.Count < columnCount)
            {
                // پر کردن سلول‌های خالی در صورت کمبود
                rowCells.AddRange(Enumerable.Repeat(string.Empty, columnCount - rowCells.Count));
            }

            markdownTable.Append(value: '|');
            markdownTable.AppendJoin(separator: '|', rowCells.Select(EscapeMarkdownTableCell));
            markdownTable.AppendLine(value: "|");
        }

        return markdownTable.ToString();
    }

    public static string EscapeMarkdownTableCell(this string? cell)
        => cell.EscapeMarkdownV2()?.Replace(oldValue: "\n", newValue: "<br>", StringComparison.Ordinal) ?? string.Empty;

    public static string? EscapeMarkdownV2([NotNullIfNotNull(nameof(text))] this string? text)
        => text.IsEmpty() ? text : MarkdownV2().Replace(text, replacement: @"\$1");
}
