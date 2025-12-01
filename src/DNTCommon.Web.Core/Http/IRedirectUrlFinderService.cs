namespace DNTCommon.Web.Core;

/// <summary>
///     Redirect Url Finder Service
/// </summary>
public interface IRedirectUrlFinderService
{
    /// <summary>
    ///     Finds the actual hidden URL after multiple redirects.
    /// </summary>
    Task<string?> GetRedirectUrlAsync(string siteUrl,
        int maxRedirects = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds the actual hidden URL after multiple redirects.
    /// </summary>
    Task<Uri?> GetRedirectUrlAsync(Uri siteUri, int maxRedirects = 20, CancellationToken cancellationToken = default);
}
