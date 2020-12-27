using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Html Helper Service
    /// </summary>
    public interface IHtmlHelperService
    {
        /// <summary>
        /// Returns the src list of img tags.
        /// </summary>
        IEnumerable<string> ExtractImagesLinks(string html);

        /// <summary>
        /// Returns the href list of anchor tags.
        /// </summary>
        IEnumerable<string> ExtractLinks(string html);

        /// <summary>
        /// Parses an HTML content and tries to convert its relative URLs to absolute urls based on the siteBaseUrl.
        /// </summary>
        string FixRelativeUrls(string html, string imageNotFoundPath, string siteBaseUrl);

        /// <summary>
        /// Parses an HTML content and tries to convert its relative URLs to absolute urls based on the siteBaseUrl.
        /// </summary>
        string FixRelativeUrls(string html, string imageNotFoundPath);

        /// <summary>
        /// Download the given uri and then extracts its title.
        /// </summary>
        Task<string> GetUrlTitleAsync(Uri uri);

        /// <summary>
        /// Download the given uri and then extracts its title.
        /// </summary>
        Task<string> GetUrlTitleAsync(string url);

        /// <summary>
        /// Extracts the given HTML page's title.
        /// </summary>
        string GetHtmlPageTitle(string html);

        /// <summary>
        /// Removes all of the HTML tags.
        /// </summary>
        string RemoveHtmlTags(string html);
    }
}