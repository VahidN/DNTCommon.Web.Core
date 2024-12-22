namespace DNTCommon.Web.Core;

/// <summary>
///     Html Helper Service
/// </summary>
public interface IHtmlHelperService
{
    /// <summary>
    ///     Returns the src list of img tags.
    /// </summary>
    IEnumerable<string> ExtractImagesLinks(string html);

    /// <summary>
    ///     Returns the href list of anchor tags.
    /// </summary>
    IEnumerable<string> ExtractLinks(string html);

    /// <summary>
    ///     Parses an HTML content and tries to convert its relative URLs to absolute urls based on the siteBaseUrl.
    /// </summary>
    string FixRelativeUrls(string html, string imageNotFoundPath, string siteBaseUrl);

    /// <summary>
    ///     Parses an HTML content and tries to convert its relative URLs to absolute urls based on the siteBaseUrl.
    /// </summary>
    string FixRelativeUrls(string html, string imageNotFoundPath);

    /// <summary>
    ///     Download the given uri and then extracts its title.
    /// </summary>
    Task<string> GetUrlTitleAsync(Uri uri);

    /// <summary>
    ///     Download the given uri and then extracts its title.
    /// </summary>
    Task<string> GetUrlTitleAsync(string url);

    /// <summary>
    ///     Extracts the given HTML page's title.
    /// </summary>
    string GetHtmlPageTitle(string html);

    /// <summary>
    ///     Removes all of the HTML tags.
    /// </summary>
    string RemoveHtmlTags(string html);

    /// <summary>
    ///     An enhanced version of HttpUtility.HtmlEncode method
    /// </summary>
    string? FullHtmlEncode(string? text);

    /// <summary>
    ///     imageBuilder delegate gives you an image's src, and then you can return its equivalent data bytes to be inserted as
    ///     an embedded data:image
    /// </summary>
    string ReplaceImageUrlsWithEmbeddedDataImages(string html, Func<string, byte[]?> imageBuilder);
}