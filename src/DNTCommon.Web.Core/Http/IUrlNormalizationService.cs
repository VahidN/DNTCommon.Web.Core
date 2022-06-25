using System;
using System.Threading.Tasks;

namespace DNTCommon.Web.Core;

/// <summary>
/// Url Normalization Service
/// </summary>
public interface IUrlNormalizationService
{
    /// <summary>
    /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
    /// </summary>
    Task<bool> AreTheSameUrlsAsync(string url1, string url2, bool findRedirectUrl);

    /// <summary>
    /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
    /// </summary>
    Task<bool> AreTheSameUrlsAsync(Uri uri1, Uri uri2, bool findRedirectUrl);

    /// <summary>
    /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
    /// https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    Task<string> NormalizeUrlAsync(Uri uri, bool findRedirectUrl);

    /// <summary>
    /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
    /// https://en.wikipedia.org/wiki/URL_normalization
    /// </summary>
    Task<string> NormalizeUrlAsync(string url, bool findRedirectUrl);
}