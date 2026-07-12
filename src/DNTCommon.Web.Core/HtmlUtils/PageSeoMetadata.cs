namespace DNTCommon.Web.Core;

/// <summary>
///     Data transfer object for SEO metadata
/// </summary>
public class PageSeoMetadata
{
    /// <summary>
    ///     Title of the document
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     URL of the largest image of the document
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    ///     Description of the largest image of the document
    /// </summary>
    public string? ImageDescription { get; set; }

    /// <summary>
    ///     Name of the site
    /// </summary>
    public string? SiteName { get; set; }

    /// <summary>
    ///     Description of the document
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Address of the sitemap
    /// </summary>
    public string? SiteMapUrl { get; set; }

    /// <summary>
    ///     Address of the RSS feed
    /// </summary>
    public string? RssUrl { get; set; }

    /// <summary>
    ///     Show llms.txt meta tags (https://llmstxt.org/)
    /// </summary>
    public bool ShowLlmsTxt { get; set; } = true;

    /// <summary>
    ///     Name of the writer of the document
    /// </summary>
    public string? AuthorName { get; set; }

    /// <summary>
    ///     Average rating value of the article
    /// </summary>
    public decimal? AverageRating { get; set; }

    /// <summary>
    ///     Number of users who have voted for the article
    /// </summary>
    public int? TotalRaters { get; set; }

    /// <summary>
    ///     Canonical URL of the document
    /// </summary>
    public string? CanonicalUrl { get; set; }

    /// <summary>
    ///     Your Twitter Handle starting with @
    /// </summary>
    public string? YourTwitterHandle { get; set; }

    /// <summary>
    ///     The OpenSearch URL
    /// </summary>
    public string? OpenSearchUrl { get; set; }

    /// <summary>
    ///     Publishing date of the article
    /// </summary>
    public DateTime? DatePublished { get; set; }

    /// <summary>
    ///     Modification date of the article
    /// </summary>
    public DateTime? DateModified { get; set; }

    /// <summary>
    ///     Tags/keywords for the article
    /// </summary>
    public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
}
