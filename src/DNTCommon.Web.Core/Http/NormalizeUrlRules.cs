namespace DNTCommon.Web.Core;

/// <summary>
///     Normalize Url Rules
/// </summary>
[Flags]
public enum NormalizeUrlRules : long
{
    /// <summary>
    ///     None
    /// </summary>
    None = 0,

    /// <summary>
    /// </summary>
    UrlToLower = 1 << 0,

    /// <summary>
    /// </summary>
    LimitProtocols = 1 << 1,

    /// <summary>
    /// </summary>
    RemoveDefaultDirectoryIndexes = 1 << 2,

    /// <summary>
    /// </summary>
    RemoveTheFragment = 1 << 3,

    /// <summary>
    /// </summary>
    RemoveDuplicateSlashes = 1 << 4,

    /// <summary>
    /// </summary>
    AddWww = 1 << 5,

    /// <summary>
    /// </summary>
    RemoveFeedburnerPart1 = 1 << 6,

    /// <summary>
    /// </summary>
    RemoveFeedburnerPart2 = 1 << 7,

    /// <summary>
    /// </summary>
    RemoveTrailingSlashAndEmptyQuery = 1 << 8,

    /// <summary>
    /// </summary>
    All = ~(~0 << 9)
}