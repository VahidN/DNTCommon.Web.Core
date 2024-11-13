namespace DNTCommon.Web.Core;

/// <summary>
///     Represents a Sitemap entry
/// </summary>
public class SitemapItem
{
    /// <summary>
    ///     Item's absolute URL
    /// </summary>
    public string Url { set; get; } = default!;

    /// <summary>
    ///     Item's Last Updated Time
    /// </summary>
    public DateTimeOffset LastUpdatedTime { set; get; }

    /// <summary>
    ///     Change frequency. Its default value is `daily`.
    /// </summary>
    public ChangeFrequency ChangeFrequency { set; get; } = ChangeFrequency.Daily;

    /// <summary>
    ///     Item's priority. Its default value is `0.5`.
    /// </summary>
    public decimal Priority { set; get; } = 0.5M;
}