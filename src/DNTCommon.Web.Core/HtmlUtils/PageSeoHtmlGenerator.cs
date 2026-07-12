using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Generates HTML SEO metadata tags for static HTML file generation
/// </summary>
public static class PageSeoHtmlGenerator
{
    /// <summary>
    ///     Generates SEO head tags as an HTML string
    /// </summary>
    public static string GenerateSeoHeadTags(this PageSeoMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        var html = new StringBuilder();

        // Keywords
        var keywords = GetKeywords(metadata.Tags);

        if (!string.IsNullOrWhiteSpace(keywords))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta name="keywords" content="{keywords.EscapeHtmlAttribute()}"/>""");
        }

        // Last Modified
        var lastModified = FormatDate(metadata.DateModified, format: "R");

        if (!string.IsNullOrWhiteSpace(lastModified))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta http-equiv="last-modified" content="{lastModified.EscapeHtmlAttribute()}"/>""");
        }

        // Description
        if (!string.IsNullOrWhiteSpace(metadata.Description))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta name="description" content="{metadata.Description.EscapeHtmlAttribute()}"/>""");
        }

        // Author
        if (!string.IsNullOrWhiteSpace(metadata.AuthorName))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta name="author" content="{metadata.AuthorName.EscapeHtmlAttribute()}"/>""");
        }

        // Title - Schema.org
        if (!string.IsNullOrWhiteSpace(metadata.Title))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta itemprop="name" content="{metadata.Title.EscapeHtmlAttribute()}"/>""");
        }

        // Description - Schema.org
        if (!string.IsNullOrWhiteSpace(metadata.Description))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta itemprop="description" content="{metadata.Description.EscapeHtmlAttribute()}"/>""");
        }

        // Sitemap
        if (!string.IsNullOrWhiteSpace(metadata.SiteMapUrl))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <link rel="sitemap" type="application/xml" title="Sitemap" href="{metadata.SiteMapUrl.EscapeHtmlAttribute()}"/>""");
        }

        // RSS Feed
        if (!string.IsNullOrWhiteSpace(metadata.RssUrl))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <link rel="alternate" type="application/rss+xml" href="{metadata.RssUrl.EscapeHtmlAttribute()}"/>""");
        }

        // Canonical URL
        if (!string.IsNullOrWhiteSpace(metadata.CanonicalUrl))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <link rel="canonical" href="{metadata.CanonicalUrl.EscapeHtmlAttribute()}"/>""");
        }

        // Robots
        html.AppendLine(value: """    <meta name="robots" content="index, follow"/>""");

        // LLMs.txt
        if (metadata.ShowLlmsTxt)
        {
            html.AppendLine(value: """    <meta name="llms.txt" content="llms.txt">""");
            html.AppendLine(value: """    <meta name="llms-full.txt" content="llms-full.txt">""");
        }

        // Structured Data (JSON-LD) - Article with Rating
        if (HasRating(metadata))
        {
            html.AppendLine(value: """    <script type="application/ld+json">""");
            html.AppendLine(GenerateJsonLd(metadata));
            html.AppendLine(value: "    </script>");
        }

        // Open Graph Tags
        if (!string.IsNullOrWhiteSpace(metadata.Title))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta property="og:title" content="{metadata.Title.EscapeHtmlAttribute()}"/>""");
        }

        if (!string.IsNullOrWhiteSpace(metadata.Description))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta property="og:description" content="{metadata.Description.EscapeHtmlAttribute()}"/>""");
        }

        if (!string.IsNullOrWhiteSpace(metadata.ImageUrl))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta property="og:image" content="{metadata.ImageUrl.EscapeHtmlAttribute()}"/>""");
        }

        if (!string.IsNullOrWhiteSpace(metadata.ImageDescription))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta property="og:image:alt" content="{metadata.ImageDescription.EscapeHtmlAttribute()}"/>""");
        }

        html.AppendLine(value: """    <meta property="og:type" content="article"/>""");

        if (!string.IsNullOrWhiteSpace(metadata.CanonicalUrl))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta property="og:url" content="{metadata.CanonicalUrl.EscapeHtmlAttribute()}"/>""");
        }

        if (!string.IsNullOrWhiteSpace(metadata.SiteName))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta property="og:site_name" content="{metadata.SiteName.EscapeHtmlAttribute()}"/>""");
        }

        // Twitter Tags
        if (!string.IsNullOrWhiteSpace(metadata.Title))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta name="twitter:title" content="{metadata.Title.EscapeHtmlAttribute()}"/>""");
        }

        if (!string.IsNullOrWhiteSpace(metadata.Description))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta name="twitter:description" content="{metadata.Description.EscapeHtmlAttribute()}"/>""");
        }

        if (!string.IsNullOrWhiteSpace(metadata.ImageUrl))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta name="twitter:image" content="{metadata.ImageUrl.EscapeHtmlAttribute()}"/>""");

            if (!string.IsNullOrWhiteSpace(metadata.ImageDescription))
            {
                html.AppendLine(CultureInfo.InvariantCulture,
                    $"""    <meta name="twitter:image:alt" content="{metadata.ImageDescription.EscapeHtmlAttribute()}"/>""");
            }
        }

        if (!string.IsNullOrWhiteSpace(metadata.Description))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta name="twitter:card" content="{metadata.Description.EscapeHtmlAttribute()}"/>""");
        }

        if (!string.IsNullOrWhiteSpace(metadata.YourTwitterHandle))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta name="twitter:site" content="{metadata.YourTwitterHandle.EscapeHtmlAttribute()}"/>""");

            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta name="twitter:creator" content="{metadata.YourTwitterHandle.EscapeHtmlAttribute()}"/>""");
        }

        // Article Publish/Modified Dates
        var pubDate = FormatDate(metadata.DatePublished, format: "R");

        if (!string.IsNullOrWhiteSpace(pubDate))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta property="article:published_time" content="{pubDate.EscapeHtmlAttribute()}"/>""");
        }

        if (!string.IsNullOrWhiteSpace(lastModified))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <meta property="article:modified_time" content="{lastModified.EscapeHtmlAttribute()}"/>""");
        }

        html.AppendLine(value: """    <meta property="article:section" content="article"/>""");
        html.AppendLine(value: """    <meta property="article:tag" content="article"/>""");

        // OpenSearch
        if (!string.IsNullOrWhiteSpace(metadata.OpenSearchUrl) && !string.IsNullOrWhiteSpace(metadata.SiteName))
        {
            html.AppendLine(CultureInfo.InvariantCulture,
                $"""    <link rel="search" type="application/opensearchdescription+xml" title="{metadata.SiteName.EscapeHtmlAttribute()}" href="{metadata.OpenSearchUrl.EscapeHtmlAttribute()}"/>""");
        }

        return html.ToString();
    }

    /// <summary>
    ///     Generates JSON-LD structured data for an article with rating
    /// </summary>
    private static string GenerateJsonLd(PageSeoMetadata metadata)
    {
        var datePublished = metadata.DatePublished?.ToString(format: "yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "";

        var json = new StringBuilder();
        json.AppendLine(value: "        {");
        json.AppendLine(value: """          "@context": "https://schema.org/",""");
        json.AppendLine(value: """          "@type": "Article",""");

        json.AppendLine(CultureInfo.InvariantCulture,
            $"""          "headline": "{(metadata.Title ?? "").EscapeJsonString()}",""");

        json.AppendLine(value: "          \"author\": {");
        json.AppendLine(value: """             "@type": "Person",""");

        json.AppendLine(CultureInfo.InvariantCulture, $"""
                                                                    "name": "{(metadata.AuthorName ?? "").EscapeJsonString()}"
                                                       """);

        json.AppendLine(value: "          },");
        json.AppendLine(CultureInfo.InvariantCulture, $"""          "datePublished": "{datePublished}",""");

        json.AppendLine(CultureInfo.InvariantCulture,
            $"""          "description": "{(metadata.Description ?? "").EscapeJsonString()}",""");

        json.AppendLine(CultureInfo.InvariantCulture,
            $"""          "image": "{(metadata.ImageUrl ?? "").EscapeJsonString()}",""");

        json.AppendLine(value: "          \"aggregateRating\": {");
        json.AppendLine(value: """             "@type": "AggregateRating",""");
        json.AppendLine(CultureInfo.InvariantCulture, $"""             "ratingValue": {metadata.AverageRating},""");
        json.AppendLine(CultureInfo.InvariantCulture, $"""             "reviewCount": {metadata.TotalRaters}""");
        json.AppendLine(value: "          }");
        json.Append(value: "        }");

        return json.ToString();
    }

    /// <summary>
    ///     Converts tags list to comma-separated keywords string
    /// </summary>
    private static string GetKeywords(IReadOnlyList<string>? tags)
        => tags is not { Count: not 0 } ? "" : string.Join(separator: ", ", tags);

    /// <summary>
    ///     Formats a DateTime to RFC1123 format (HTTP date format)
    /// </summary>
    private static string? FormatDate(DateTime? date, string format)
        => date?.ToUniversalTime().ToString(format, CultureInfo.InvariantCulture);

    /// <summary>
    ///     Checks if article has rating
    /// </summary>
    private static bool HasRating(PageSeoMetadata metadata) => metadata is { AverageRating: > 0, TotalRaters: > 0 };

    /// <summary>
    ///     Escapes HTML special characters for use in attribute values
    /// </summary>
    public static string EscapeHtmlAttribute(this string? value)
        => string.IsNullOrEmpty(value)
            ? ""
            : value.Replace(oldValue: "&", newValue: "&amp;", StringComparison.Ordinal)
                .Replace(oldValue: "<", newValue: "&lt;", StringComparison.Ordinal)
                .Replace(oldValue: ">", newValue: "&gt;", StringComparison.Ordinal)
                .Replace(oldValue: "\"", newValue: "&quot;", StringComparison.Ordinal)
                .Replace(oldValue: "'", newValue: "&#39;", StringComparison.Ordinal);

    /// <summary>
    ///     Escapes special characters for JSON strings
    /// </summary>
    public static string EscapeJsonString(this string? value)
        => string.IsNullOrEmpty(value)
            ? ""
            : value.Replace(oldValue: "\\", newValue: "\\\\", StringComparison.Ordinal)
                .Replace(oldValue: "\"", newValue: "\\\"", StringComparison.Ordinal)
                .Replace(oldValue: "\n", newValue: "\\n", StringComparison.Ordinal)
                .Replace(oldValue: "\r", newValue: "\\r", StringComparison.Ordinal)
                .Replace(oldValue: "\t", newValue: "\\t", StringComparison.Ordinal);
}
