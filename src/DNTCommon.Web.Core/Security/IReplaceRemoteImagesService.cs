using HtmlAgilityPack;

namespace DNTCommon.Web.Core;

/// <summary>
///     A helper method to download and fix remote images linked from other sites
/// </summary>
public interface IReplaceRemoteImagesService
{
    /// <summary>
    ///     A helper method to download and fix remote images linked from other sites
    /// </summary>
    void FixRemoteImages(HtmlNode? node, FixRemoteImagesOptions? options);
}