#if !NET_6
using System.Text;
using System.Text.RegularExpressions;

namespace DNTCommon.Web.Core;

public static partial class GeminiOutputNormalizer
{
    private const string CodeHighlightingLineSeperator = " $$CHLS$$ ";

    [GeneratedRegex(pattern: @"!\[(.*?)\]\((.*?)(\s*""[^""]*"")?(\s*=\s*(\d*)x?(\d*))?\)",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 3000)]
    private static partial Regex ImageRegex();

    [GeneratedRegex(pattern: @"^\s*\d+\.\s", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex PatternOrderedListRegex();

    [GeneratedRegex(pattern: @"^\s*\-\s", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex PatternUnorderedListRegex();

    [GeneratedRegex(pattern: @"\```(\w+)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex Code1Regex();

    [GeneratedRegex(pattern: @"```", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex Code2Regex();

    [GeneratedRegex(pattern: @"\*\*(.*?)\*\*",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 3000)]
    private static partial Regex Emphasis1Regex();

    [GeneratedRegex(pattern: @"__(.*?)__", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex Emphasis2Regex();

    [GeneratedRegex(pattern: @"\*(.*?)\*", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex Emphasis3Regex();

    [GeneratedRegex(pattern: @"_(.*?)_", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex Emphasis4Regex();

    [GeneratedRegex(pattern: @"~~(.*?)~~", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex Emphasis5Regex();

    [GeneratedRegex(pattern: @"^#{6}\s?([^\n]+)",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 3000)]
    private static partial Regex Header6Regex();

    [GeneratedRegex(pattern: @"^#{5}\s?([^\n]+)",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 3000)]
    private static partial Regex Header5Regex();

    [GeneratedRegex(pattern: @"^#{4}\s?([^\n]+)",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 3000)]
    private static partial Regex Header4Regex();

    [GeneratedRegex(pattern: @"^#{3}\s?([^\n]+)",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 3000)]
    private static partial Regex Header3Regex();

    [GeneratedRegex(pattern: @"^#{2}\s?([^\n]+)",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 3000)]
    private static partial Regex Header2Regex();

    [GeneratedRegex(pattern: @"^#{1}\s?([^\n]+)",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 3000)]
    private static partial Regex Header1Regex();

    [GeneratedRegex(pattern: @"`([^`]+)`", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex SingleLinePattern();

    [GeneratedRegex(pattern: @"\[(.*?)\]\((.*?)\)",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 3000)]
    private static partial Regex MarkdownLinkPattern();

    [GeneratedRegex(pattern: @"^>{1,}\s(.*)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex MarkdownBLOCKQUOTESPattern();

    [GeneratedRegex(pattern: @"^\-{3,}$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex MarkdownHorizontalRulesPattern();

    public static string ConvertMarkdownToHtml(this string? text) => text.GetNormalizedAIText();

    public static string GetNormalizedAIText(this string? text)
    {
        if (text is null)
        {
            return string.Empty;
        }

        var markup = text.GetMarkup();
        markup = markup.ConvertMakdownHeadersToHtml();
        markup = markup.ConvertMarkdownBlockquotesToHtml();
        markup = markup.ConvertMarkdownHorizontalRulesToHtml();
        markup = markup.ConvertMarkdownEmphasisToHtml();
        markup = markup.ConvertMarkdownCodeHighlightingToHtml();
        markup = markup.ConvertMarkdownListToHtml();
        markup = markup.ConvertMarkdownTableToHtml();
        markup = markup.ConvertMarkdownParagraphsToHtml();
        markup = markup.ConvertMarkdownLineBreaksToHtml();
        markup = markup.ConvertMarkdownImageToHtml();
        markup = markup.ConvertMarkdownLinksToHtml();
        markup = markup.ConvertMarkdownInlineCodeToHtml();

        markup = markup.Replace(CodeHighlightingLineSeperator, newValue: "\n", StringComparison.Ordinal);

        return markup;
    }

    public static string ConvertMarkdownLineBreaksToHtml(this string? markup)
        => markup.IsEmpty()
            ? string.Empty
            : markup.Replace(oldValue: "\n", newValue: "<br />", StringComparison.OrdinalIgnoreCase);

    public static string ConvertMarkdownImageToHtml(this string? markup)
    {
        if (markup.IsEmpty())
        {
            return string.Empty;
        }

        // Pattern to match Markdown image syntax: ![alt text](url "optional title" =WIDTHxHEIGHT)
        // Replace Markdown image syntax with HTML <img> tag
        var html = ImageRegex()
            .Replace(markup, match =>
            {
                var altText = match.Groups[groupnum: 1].Value;
                var url = match.Groups[groupnum: 2].Value;
                var title = match.Groups[groupnum: 3].Value;
                var width = match.Groups[groupnum: 5].Value;
                var height = match.Groups[groupnum: 6].Value;

                var imgTag = $"<img src=\"{url}\" alt=\"{altText}\"";

                if (!string.IsNullOrEmpty(title))
                {
                    imgTag += $" title={title}";
                }

                if (!string.IsNullOrEmpty(width))
                {
                    imgTag += $" width=\"{width}\"";
                }

                if (!string.IsNullOrEmpty(height))
                {
                    imgTag += $" height=\"{height}\"";
                }

                imgTag += " />";

                return imgTag;
            });

        return html;
    }

    public static string ConvertMarkdownParagraphsToHtml(this string? markup)
    {
        if (markup.IsEmpty())
        {
            return string.Empty;
        }

        var lines = markup.Split(separator: "\n\n\n");
        var parsedLines = new List<string>();

        if (lines.Length == 1)
        {
            return markup;
        }

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                parsedLines.Add(line);

                continue;
            }

            parsedLines.Add($"<p>{line}</p>");
        }

        return string.Concat(parsedLines);
    }

    public static string ConvertMarkdownTableToHtml(this string? markup)
    {
        if (markup.IsEmpty())
        {
            return string.Empty;
        }

        var lines = markup.Split(separator: "\n");
        var parsedLines = new List<string>();
        var htmlLines = new List<string>();

        var isTableStart = false;
        var isTableHeadingAdded = false;

        // Read lines starting with '|'
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                parsedLines.Add(line);

                continue;
            }

            // Trim row with spaces
            var trimmedLine = line.Trim();

            if (trimmedLine.StartsWith(value: "| ", StringComparison.OrdinalIgnoreCase))
            {
                if (!isTableStart)
                {
                    isTableStart = true;
                }

                // Remove '|' from the start and end of the line
                trimmedLine = trimmedLine.TrimStart(trimChar: '|').TrimEnd(trimChar: '|');

                // Convert MD syntax to HTML
                var cells = trimmedLine.Split(separator: "|", StringSplitOptions.RemoveEmptyEntries);
                var tableRow = new StringBuilder();
                tableRow.Append(value: "<tr>");

                foreach (var cell in cells)
                {
                    var tableCellTagName = !isTableHeadingAdded ? "th" : "td";
                    var tableCell = $"<{tableCellTagName}>{cell.Trim()}</{tableCellTagName}>";
                    tableRow.Append(tableCell);
                }

                tableRow.Append(value: "</tr>");
                htmlLines.Add(tableRow.ToString());
            }
            else if (trimmedLine.StartsWith(value: "|--", StringComparison.OrdinalIgnoreCase) ||
                     trimmedLine.StartsWith(value: "|:--", StringComparison.OrdinalIgnoreCase))
            {
                // Table heading row is over
                if (!isTableHeadingAdded)
                {
                    isTableHeadingAdded = true;
                    htmlLines.Add(item: "</thead>");
                    htmlLines.Add(item: "<tbody>");
                }
            }
            else if (isTableStart)
            {
                isTableStart = false;

                parsedLines.Add($"<table><thead>{string.Concat(htmlLines)}</tbody></table>");

                htmlLines.Clear();
            }
            else
            {
                parsedLines.Add(line);
            }
        }

        if (isTableStart && htmlLines.Count != 0)
        {
            parsedLines.Add($"<table><thead>{string.Concat(htmlLines)}</tbody></table>");
            htmlLines.Clear();
        }

        return string.Join(separator: '\n', parsedLines);
    }

    private static string GetMarkup(this string text)
    {
        text = WebUtility.HtmlDecode(text.Trim());

        var lines = text.GetLines().ToList();

        if (lines.Count > 0)
        {
            // remove first blank line
            if (string.IsNullOrWhiteSpace(lines[index: 0]))
            {
                lines.RemoveAt(index: 0);
            }

            // remove last blank line
            if (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1]))
            {
                lines.RemoveAt(lines.Count - 1);
            }
        }

        return string.Join(separator: '\n', lines);
    }

    public static string ConvertMarkdownCodeHighlightingToHtml(this string? markup)
    {
        if (markup.IsEmpty())
        {
            return string.Empty;
        }

        var lines = markup.Split(separator: "\n");
        var parsedLines = new List<string>();
        var isCodeBlockInprogress = false;

        for (var i = 0; i < lines.Count(); i++)
        {
            if (Code1Regex().IsMatch(lines[i].Trim()))
            {
                if (!isCodeBlockInprogress)
                {
                    isCodeBlockInprogress = true;
                }

                lines[i] = Code1Regex().Replace(lines[i].Trim(), replacement: "<pre><code class=\"lang-$1\">");

                parsedLines.Add(lines[i]);
            }
            else if (Code2Regex().IsMatch(lines[i].Trim().Trim()))
            {
                if (isCodeBlockInprogress)
                {
                    isCodeBlockInprogress = false;
                }

                lines[i] = Code2Regex().Replace(lines[i].Trim(), replacement: "</code></pre>");
                parsedLines.Add(lines[i]);
            }
            else if (isCodeBlockInprogress)
            {
                parsedLines.Add(lines[i]);
                parsedLines.Add(CodeHighlightingLineSeperator);
            }
            else
            {
                parsedLines.Add(lines[i]);
                parsedLines.Add(item: "\n");
            }
        }

        parsedLines.RemoveLastLineBreak();

        return string.Concat(parsedLines);
    }

    public static string ConvertMarkdownInlineCodeToHtml(this string? text)
    {
        if (text.IsEmpty())
        {
            return string.Empty;
        }

        text = SingleLinePattern()
            .Replace(text, m =>
            {
                var code = m.Groups[groupnum: 1].Value;
                var safeCode = WebUtility.HtmlEncode(code);

                return $"<code dir='ltr'>{safeCode}</code>";
            });

        return text;
    }

    public static string ConvertMarkdownLinksToHtml(this string? text)
    {
        if (text.IsEmpty())
        {
            return string.Empty;
        }

        text = MarkdownLinkPattern()
            .Replace(text, match =>
            {
                var linkText = match.Groups[groupnum: 1].Value;
                var linkUrl = match.Groups[groupnum: 2].Value;

                return $"<a href=\"{linkUrl}\">{linkText}</a>";
            });

        return text;
    }

    public static string ConvertMarkdownBlockquotesToHtml(this string? markup)
    {
        if (markup.IsEmpty())
        {
            return string.Empty;
        }

        var lines = markup.Split(separator: "\n");
        var htmlLines = new List<string>();
        var listStack = new Stack<string>();
        var indentStack = new Stack<int>();

        foreach (var line in lines)
        {
            var trimmerLine = line.Trim();
            var indentLevel = trimmerLine.TakeWhile(c => c == '>').Count();

            if (MarkdownBLOCKQUOTESPattern().IsMatch(trimmerLine))
            {
                if (listStack.Count == 0 || indentStack.Peek() < indentLevel)
                {
                    if (listStack.Count > 0 && indentStack.Peek() < indentLevel)
                    {
                        // close the `p` tag
                        if (listStack.Peek() == "p")
                        {
                            htmlLines.Add($"</{listStack.Pop()}>");
                        }

                        htmlLines.Add(item: "<blockquote>");
                        listStack.Push(item: "blockquote");
                        indentStack.Push(indentLevel);
                    }
                    else
                    {
                        while (listStack.Count > 0 && indentStack.Peek() > indentLevel)
                        {
                            htmlLines.Add($"</{listStack.Pop()}>");
                            indentStack.Pop();
                        }

                        if (listStack.Count == 0 || listStack.Peek() != "blockquote")
                        {
                            htmlLines.Add(item: "<blockquote>");
                            listStack.Push(item: "blockquote");
                            indentStack.Push(indentLevel);
                        }
                    }

                    htmlLines.Add($"<p>{MarkdownBLOCKQUOTESPattern().Replace(trimmerLine, replacement: "$1")}");
                    listStack.Push(item: "p");
                }
                else if (indentStack.Peek() >= indentLevel)
                {
                    htmlLines.Add($"<br />{MarkdownBLOCKQUOTESPattern().Replace(trimmerLine, replacement: "$1")}");
                }
            }
            else
            {
                // Close any open lists
                while (listStack.Count > 0)
                {
                    htmlLines.Add($"</{listStack.Pop()}>");

                    if (indentStack.Count > 0)
                    {
                        indentStack.Pop();
                    }
                }

                htmlLines.Add(line);
                htmlLines.Add(item: "\n");
            }
        }

        // Close any remaining open blockquotes
        while (listStack.Count > 0)
        {
            htmlLines.Add($"</{listStack.Pop()}>");

            if (indentStack.Count > 0)
            {
                indentStack.Pop();
            }
        }

        return string.Concat(htmlLines);
    }

    public static string ConvertMakdownHeadersToHtml(this string? markup)
    {
        if (markup.IsEmpty())
        {
            return string.Empty;
        }

        var lines = markup.Split(separator: "\n");
        var parsedLines = new List<string>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                parsedLines.Add(line);
                parsedLines.Add(item: "\n");

                continue;
            }

            if (Header6Regex().IsMatch(line.Trim()))
            {
                parsedLines.Add(Header6Regex().Replace(line.Trim(), replacement: "<h6>$1</h6>"));
            }
            else if (Header5Regex().IsMatch(line.Trim()))
            {
                parsedLines.Add(Header5Regex().Replace(line.Trim(), replacement: "<h5>$1</h5>"));
            }
            else if (Header4Regex().IsMatch(line.Trim()))
            {
                parsedLines.Add(Header4Regex().Replace(line.Trim(), replacement: "<h4>$1</h4>"));
            }
            else if (Header3Regex().IsMatch(line.Trim()))
            {
                parsedLines.Add(Header3Regex().Replace(line.Trim(), replacement: "<h3>$1</h3>"));
            }
            else if (Header2Regex().IsMatch(line.Trim()))
            {
                parsedLines.Add(Header2Regex().Replace(line.Trim(), replacement: "<h2>$1</h2>"));
            }
            else if (Header1Regex().IsMatch(line.Trim()))
            {
                parsedLines.Add(Header1Regex().Replace(line.Trim(), replacement: "<h1>$1</h1>"));
            }
            else
            {
                parsedLines.Add(line);
                parsedLines.Add(item: "\n");
            }
        }

        parsedLines.RemoveLastLineBreak();

        return string.Concat(parsedLines);
    }

    public static string ConvertMarkdownHorizontalRulesToHtml(this string? markup)
    {
        if (markup.IsEmpty())
        {
            return string.Empty;
        }

        var lines = markup.Split(separator: "\n");
        var parsedLines = new List<string>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                parsedLines.Add(line);
                parsedLines.Add(item: "\n");

                continue;
            }

            if (MarkdownHorizontalRulesPattern().IsMatch(line.Trim()))
            {
                parsedLines.RemoveLastLineBreak();
                parsedLines.Add(MarkdownHorizontalRulesPattern().Replace(line.Trim(), replacement: "<hr />"));
            }
            else
            {
                parsedLines.Add(line);
                parsedLines.Add(item: "\n");
            }
        }

        parsedLines.RemoveLastLineBreak();

        return string.Concat(parsedLines);
    }

    public static string ConvertMarkdownEmphasisToHtml(this string? markup)
    {
        if (markup.IsEmpty())
        {
            return string.Empty;
        }

        var lines = markup.Split(separator: "\n");
        var parsedLines = new List<string>();

        for (var i = 0; i < lines.Count(); i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                parsedLines.Add(lines[i]);

                continue;
            }

            if (Emphasis1Regex().IsMatch(lines[i].Trim()))
            {
                lines[i] = Emphasis1Regex().Replace(lines[i].Trim(), replacement: "<b>$1</b>");
            }

            if (Emphasis2Regex().IsMatch(lines[i].Trim().Trim()))
            {
                lines[i] = Emphasis2Regex().Replace(lines[i].Trim(), replacement: "<b>$1</b>");
            }

            if (Emphasis3Regex().IsMatch(lines[i].Trim()))
            {
                lines[i] = Emphasis3Regex().Replace(lines[i].Trim(), replacement: "<i>$1</i>");
            }

            if (Emphasis4Regex().IsMatch(lines[i].Trim()))
            {
                lines[i] = Emphasis4Regex().Replace(lines[i].Trim(), replacement: "<i>$1</i>");
            }

            if (Emphasis5Regex().IsMatch(lines[i].Trim()))
            {
                lines[i] = Emphasis5Regex().Replace(lines[i].Trim(), replacement: "<s>$1</s>");
            }

            parsedLines.Add(lines[i]);
        }

        return string.Join(separator: '\n', parsedLines);
    }

    public static string ConvertMarkdownListToHtml(this string? markup)
    {
        if (markup.IsEmpty())
        {
            return string.Empty;
        }

        var lines = markup.Split(separator: "\n");
        var htmlLines = new List<string>();
        var listStack = new Stack<string>();
        var indentStack = new Stack<int>();

        foreach (var line in lines)
        {
            var indentLevel = line.TakeWhile(char.IsWhiteSpace).Count();

            if (PatternOrderedListRegex().IsMatch(line))
            {
                ProcessOrderedList(listStack, indentStack, indentLevel, htmlLines, line);
            }
            else if (PatternUnorderedListRegex().IsMatch(line))
            {
                ProcessUnorderedList(listStack, indentStack, indentLevel, htmlLines, line);
            }
            else
            {
                // Close any open lists
                while (listStack.Count > 0)
                {
                    htmlLines.Add($"</{listStack.Pop()}>");
                    indentStack.Pop();
                }

                htmlLines.Add(line);
                htmlLines.Add(item: "\n");
            }
        }

        // Close any remaining open lists
        while (listStack.Count > 0)
        {
            htmlLines.Add($"</{listStack.Pop()}>");
            indentStack.Pop();
        }

        // Close any open list items
        for (var i = 0; i < htmlLines.Count; i++)
        {
            if (htmlLines[i].StartsWith(value: "<li>", StringComparison.OrdinalIgnoreCase) &&
                (i == htmlLines.Count - 1 ||
                 htmlLines[i + 1].StartsWith(value: "<li>", StringComparison.OrdinalIgnoreCase) ||
                 htmlLines[i + 1].StartsWith(value: "</", StringComparison.OrdinalIgnoreCase)))
            {
                htmlLines[i] += "</li>";
            }
        }

        htmlLines.RemoveLastLineBreak();

        return string.Concat(htmlLines);
    }

    private static void ProcessUnorderedList(Stack<string> listStack,
        Stack<int> indentStack,
        int indentLevel,
        List<string> htmlLines,
        string line)
    {
        // Unordered list
        if (listStack.Count == 0 || listStack.Peek() != "ul" || indentStack.Peek() < indentLevel)
        {
            if (listStack.Count > 0 && indentStack.Peek() < indentLevel)
            {
                htmlLines.Add(item: "<ul>");
                listStack.Push(item: "ul");
                indentStack.Push(indentLevel);
            }
            else
            {
                while (listStack.Count > 0 && indentStack.Peek() > indentLevel)
                {
                    htmlLines.Add($"</{listStack.Pop()}>");
                    indentStack.Pop();
                }

                if (listStack.Count == 0 || listStack.Peek() != "ul")
                {
                    htmlLines.Add(item: "<ul>");
                    listStack.Push(item: "ul");
                    indentStack.Push(indentLevel);
                }
            }

            htmlLines.Add($"<li>{PatternUnorderedListRegex().Replace(line, replacement: "")}");
        }
        else if (indentStack.Peek() > indentLevel)
        {
            htmlLines.Add($"</{listStack.Pop()}>");
            indentStack.Pop();

            htmlLines.Add($"<li>{PatternUnorderedListRegex().Replace(line, replacement: "")}");
        }
        else if (indentStack.Peek() == indentLevel)
        {
            htmlLines.Add($"<li>{PatternUnorderedListRegex().Replace(line, replacement: "")}");
        }
    }

    private static void ProcessOrderedList(Stack<string> listStack,
        Stack<int> indentStack,
        int indentLevel,
        List<string> htmlLines,
        string line)
    {
        // Ordered list
        if (listStack.Count == 0 || listStack.Peek() != "ol" || indentStack.Peek() < indentLevel)
        {
            if (listStack.Count > 0 && indentStack.Peek() < indentLevel)
            {
                htmlLines.Add(item: "<ol>");
                listStack.Push(item: "ol");
                indentStack.Push(indentLevel);
            }
            else
            {
                while (listStack.Count > 0 && indentStack.Peek() > indentLevel)
                {
                    htmlLines.Add($"</{listStack.Pop()}>");
                    indentStack.Pop();
                }

                if (listStack.Count == 0 || listStack.Peek() != "ol")
                {
                    htmlLines.Add(item: "<ol>");
                    listStack.Push(item: "ol");
                    indentStack.Push(indentLevel);
                }
            }

            htmlLines.Add($"<li>{PatternOrderedListRegex().Replace(line, replacement: "")}");
        }
        else if (indentStack.Peek() > indentLevel)
        {
            htmlLines.Add($"</{listStack.Pop()}>");
            indentStack.Pop();

            htmlLines.Add($"<li>{PatternOrderedListRegex().Replace(line, replacement: "")}");
        }
        else if (indentStack.Peek() == indentLevel)
        {
            htmlLines.Add($"<li>{PatternOrderedListRegex().Replace(line, replacement: "")}");
        }
    }

    private static void RemoveLastLineBreak(this List<string> htmlLines)
    {
        // remove last line break
        if (htmlLines.Count != 0 && htmlLines[^1] == "\n")
        {
            htmlLines.RemoveAt(htmlLines.Count - 1);
        }
    }
}

#endif
