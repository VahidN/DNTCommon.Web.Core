namespace DNTCommon.Web.Core;

/// <summary>
///     Represents a feed entry
/// </summary>
public class FeedItem
{
    /// <summary>
    ///     Item's title
    /// </summary>
    public string Title { set; get; } = default!;

    /// <summary>
    ///     Item's Author Name
    /// </summary>
    public string AuthorName { set; get; } = default!;

    /// <summary>
    ///     Item's description
    /// </summary>
    public string Content { set; get; } = default!;

    /// <summary>
    ///     Item's Categories
    /// </summary>
    public IEnumerable<string> Categories { set; get; } = [];

    /// <summary>
    ///     Item's absolute URL
    /// </summary>
    public string Url { set; get; } = default!;

    /// <summary>
    ///     Item's Last Updated Time
    /// </summary>
    public DateTimeOffset LastUpdatedTime { set; get; } = default!;

    /// <summary>
    ///     Item's Publish Date
    /// </summary>
    public DateTimeOffset PublishDate { set; get; } = default!;
}