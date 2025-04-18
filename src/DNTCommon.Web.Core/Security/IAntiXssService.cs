namespace DNTCommon.Web.Core;

/// <summary>
///     Anti Xss Service
/// </summary>
public interface IAntiXssService : IDisposable
{
    /// <summary>
    ///     Takes raw HTML input and cleans against a whitelist
    /// </summary>
    /// <param name="html">Html source</param>
    /// <param name="allowDataAttributes">Allow HTML5 data attributes prefixed with data-</param>
    /// <param name="remoteImagesOptions"></param>
    /// <param name="htmlModificationRules"></param>
    /// <returns>Clean output</returns>
    string GetSanitizedHtml(string? html,
        bool allowDataAttributes = true,
        FixRemoteImagesOptions? remoteImagesOptions = null,
        HtmlModificationRules? htmlModificationRules = null);
}
