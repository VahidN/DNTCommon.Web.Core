using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Components;

namespace DNTCommon.Web.Core;

/// <summary>
///     Blazor Html Utils
/// </summary>
public static class BlazorHtmlExtensions
{
    /// <summary>
    ///     Renders the string value as a markup such as HTML.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="wrapItWithDir">Sets direction of the text based on its characters. It can be ltr or rtl.</param>
    /// <param name="wrapperElementName"></param>
    /// <returns></returns>
    public static MarkupString HtmlRaw(this string? text, bool wrapItWithDir = true, string wrapperElementName = "span")
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return (MarkupString)"";
        }

        if (wrapItWithDir)
        {
            var hasRtl = text.ContainsFarsi(allowWhitespace: true);
            var dir = hasRtl ? "rtl" : "ltr";
            var align = hasRtl ? "right" : "left";
            text = $"<{wrapperElementName} style='text-align: {align}' dir='{dir}'>{text}</{wrapperElementName}>";
        }

        return (MarkupString)text;
    }
}