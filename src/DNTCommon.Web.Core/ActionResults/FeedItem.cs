namespace DNTCommon.Web.Core;

/// <summary>
///     Represents a feed entry
/// </summary>
public class FeedItem
{
    /// <summary>
    ///     Item's title
    /// </summary>
    public string Title { set; get; } = null!;

    /// <summary>
    ///     Item's Author Name
    /// </summary>
    public string AuthorName { set; get; } = null!;

    /// <summary>
    ///     Item's description
    /// </summary>
    public string Content { set; get; } = null!;

    /// <summary>
    ///     Item's Categories
    /// </summary>
    public IEnumerable<string> Categories { set; get; } = [];

    /// <summary>
    ///     Item's absolute URL
    /// </summary>
    public string Url { set; get; } = null!;

    /// <summary>
    ///     Item's Last Updated Time
    /// </summary>
    public DateTimeOffset LastUpdatedTime { set; get; }

    /// <summary>
    ///     Item's Publish Date
    /// </summary>
    public DateTimeOffset PublishDate { set; get; }
}
