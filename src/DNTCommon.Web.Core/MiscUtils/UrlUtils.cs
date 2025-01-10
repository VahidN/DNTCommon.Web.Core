using System.Text.RegularExpressions;

namespace DNTCommon.Web.Core;

/// <summary>
///     Url Utils
/// </summary>
public static class UrlUtils
{
    /// <summary>
    ///     Tries to extract the image format form a url
    /// </summary>
    /// <param name="thumbnailUrl"></param>
    /// <returns></returns>
    public static string? TryGetImageFormat(this string? thumbnailUrl)
        => TryExtractFileName(thumbnailUrl)?.Pipe(Path.GetExtension)?.Trim(trimChar: '.');

    /// <summary>
    ///     Tries to extract a file name form a url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string? TryExtractFileName(this string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        return Regex.Match(url, pattern: @".+/([^?]*)", RegexOptions.Compiled, TimeSpan.FromMinutes(value: 1))
            .Groups[groupnum: 1]
            .Value.NullIfEmptyOrWhiteSpace();
    }

    /// <summary>
    ///     Pipes different operations
    /// </summary>
    /// <param name="input"></param>
    /// <param name="transform"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> transform)
    {
        ArgumentNullException.ThrowIfNull(transform);

        return transform(input);
    }

    /// <summary>
    ///     Allows adding params to the given URI.
    /// </summary>
    public static Uri AddQueryString(this Uri uri, string key, string value)
        => new UriBuilderExtensions(uri).AddParameter(key, value).Uri;

    /// <summary>
    ///     Allows adding params to the given URI.
    /// </summary>
    public static Uri AddQueryString(this string uri, string key, string value)
        => new UriBuilderExtensions(uri).AddParameter(key, value).Uri;

    /// <summary>
    ///     Allows adding or replacing params to the given URI.
    /// </summary>
    public static Uri AddOrUpdateQueryString(this Uri uri, string key, string value)
        => new UriBuilderExtensions(uri).AddOrUpdateParameter(key, value).Uri;

    /// <summary>
    ///     Allows adding or replacing params to the given URI.
    /// </summary>
    public static Uri AddOrUpdateQueryString(this string uri, string key, string value)
        => new UriBuilderExtensions(uri).AddOrUpdateParameter(key, value).Uri;

    /// <summary>
    ///     Allows replacing existing params to the given URI.
    /// </summary>
    public static Uri UpdateQueryString(this Uri uri, string key, string value)
        => new UriBuilderExtensions(uri).UpdateParameter(key, value).Uri;

    /// <summary>
    ///     Allows replacing existing params to the given URI.
    /// </summary>
    public static Uri UpdateQueryString(this string uri, string key, string value)
        => new UriBuilderExtensions(uri).UpdateParameter(key, value).Uri;

    /// <summary>
    ///     Allows removing params to the given URI.
    /// </summary>
    public static Uri UpdateQueryString(this Uri uri, string key)
        => new UriBuilderExtensions(uri).RemoveParameter(key).Uri;

    /// <summary>
    ///     Allows removing params to the given URI.
    /// </summary>
    public static Uri UpdateQueryString(this string uri, string key)
        => new UriBuilderExtensions(uri).RemoveParameter(key).Uri;
}