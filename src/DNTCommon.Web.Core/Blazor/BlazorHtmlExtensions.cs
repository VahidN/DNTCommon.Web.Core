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
    /// <returns></returns>
    public static MarkupString HtmlRaw(this string? text)
        => string.IsNullOrWhiteSpace(text) ? (MarkupString)"" : (MarkupString)text;
}