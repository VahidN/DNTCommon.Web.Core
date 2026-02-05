#if !NET_6
using System.Text.RegularExpressions;

namespace DNTCommon.Web.Core;

public static partial class GeminiOutputNormalizer
{
    [GeneratedRegex(pattern: @"```([\s\S]*?)```",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 3000)]
    private static partial Regex MultiLinePattern();

    [GeneratedRegex(pattern: @"`([^`]+)`", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase,
        matchTimeoutMilliseconds: 3000)]
    private static partial Regex SingleLinePattern();

    public static string GetNormalizedAIText(this string? text, bool processCodes)
    {
        if (text is null)
        {
            return string.Empty;
        }

        text = WebUtility.HtmlDecode(text.Trim());

        if (processCodes)
        {
            // ۱. ابتدا بلاک‌های چندخطی (سه بک‌تیک) را تبدیل می‌کنیم
            text = MultiLinePattern()
                .Replace(text, m =>
                {
                    var code = m.Groups[groupnum: 1].Value.Trim();
                    var safeCode = WebUtility.HtmlEncode(code);

                    return $"<pre dir='ltr'><code>{safeCode}</code></pre>";
                });

            // ۲. سپس بلاک‌های تک‌خطی (یک بک‌تیک) را تبدیل می‌کنیم
            text = SingleLinePattern()
                .Replace(text, m =>
                {
                    var code = m.Groups[groupnum: 1].Value;
                    var safeCode = WebUtility.HtmlEncode(code);

                    return $"<code dir='ltr'>{safeCode}</code>";
                });
        }

        return text;
    }
}
#endif
