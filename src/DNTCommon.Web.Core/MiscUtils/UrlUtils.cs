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
        => TryExtractFileName(thumbnailUrl)?.Pipe(Path.GetExtension)?.Trim('.');

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

        return Regex.Match(url, @".+/([^?]*)", RegexOptions.Compiled, TimeSpan.FromMinutes(1))
            .Groups[1]
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
}