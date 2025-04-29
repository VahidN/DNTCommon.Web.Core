using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace DNTCommon.Web.Core;

/// <summary>
///     HttpUtility Extensions
/// </summary>
public static class HttpUtilityExtensions
{
    /// <summary>
    ///     Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.
    /// </summary>
    [return: NotNullIfNotNull(nameof(html))]
    public static string? HtmlDecode(this string? html) => HttpUtility.HtmlDecode(html);

    /// <summary>
    ///     Converts a string that has been HTML-encoded into a decoded string, and sends the decoded string to a TextWriter
    ///     output stream.
    /// </summary>
    public static void HtmlDecode(this string? html, TextWriter output) => HttpUtility.HtmlDecode(html, output);

    /// <summary>
    ///     Converts a string to an HTML-encoded string.
    /// </summary>
    [return: NotNullIfNotNull(nameof(html))]
    public static string? HtmlEncode(this string? html) => HttpUtility.HtmlEncode(html);

    /// <summary>
    ///     Converts a string into an HTML-encoded string, and returns the output as a TextWriter stream of output.
    /// </summary>
    public static void HtmlEncode(this string? html, TextWriter output) => HttpUtility.HtmlEncode(html, output);

    /// <summary>
    ///     Encodes a string.
    /// </summary>
    public static string JavaScriptStringEncode(this string? value) => HttpUtility.JavaScriptStringEncode(value);

    /// <summary>
    ///     Encodes a string.
    /// </summary>
    public static string JavaScriptStringEncode(this string? value, bool addDoubleQuotes)
        => HttpUtility.JavaScriptStringEncode(value, addDoubleQuotes);

    /// <summary>
    ///     Parses a query string into a NameValueCollection using UTF8 encoding.
    /// </summary>
    public static NameValueCollection ParseQueryString(this string query) => HttpUtility.ParseQueryString(query);

    /// <summary>
    ///     Parses a query string into a NameValueCollection using the specified Encoding.
    /// </summary>
    public static NameValueCollection ParseQueryString(this string query, Encoding encoding)
        => HttpUtility.ParseQueryString(query, encoding);

    /// <summary>
    ///     Converts a string that has been encoded for transmission in a URL into a decoded string.
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? UrlDecode(this string? text) => HttpUtility.UrlDecode(text);

    /// <summary>
    ///     Converts a URL-encoded string into a decoded string, using the specified encoding object.
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? UrlDecode(this string? text, Encoding e) => HttpUtility.UrlDecode(text, e);

    /// <summary>
    ///     Converts a URL-encoded string into a decoded array of bytes.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(text))]
    public static byte[]? UrlDecodeToBytes(this string? text) => HttpUtility.UrlDecodeToBytes(text);

    /// <summary>
    ///     Converts a URL-encoded string into a decoded array of bytes using the specified decoding object.
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static byte[]? UrlDecodeToBytes(this string? text, Encoding e) => HttpUtility.UrlDecodeToBytes(text, e);

    /// <summary>
    ///     Encodes a URL string.
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? UrlEncode(this string? text) => HttpUtility.UrlEncode(text);

    /// <summary>
    ///     Encodes a URL string using the specified encoding object.
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? UrlEncode(this string? text, Encoding e) => HttpUtility.UrlEncode(text, e);

    /// <summary>
    ///     Converts a string into a URL-encoded array of bytes.
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static byte[]? UrlEncodeToBytes(this string? text) => HttpUtility.UrlEncodeToBytes(text);

    /// <summary>
    ///     Converts a string into a URL-encoded array of bytes using the specified encoding object.
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static byte[]? UrlEncodeToBytes(this string? text, Encoding e) => HttpUtility.UrlEncodeToBytes(text, e);

    /// <summary>
    ///     Do not use; intended only for browser compatibility. Use UrlEncode(string?).
    /// </summary>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? UrlPathEncode(this string? text) => HttpUtility.UrlPathEncode(text);
}
