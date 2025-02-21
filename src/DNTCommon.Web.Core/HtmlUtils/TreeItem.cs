namespace DNTCommon.Web.Core;

/// <summary>
///     Represents a TreeItem in a tree-view
/// </summary>
[DebuggerDisplay(value: "{Id}{ParentItemId}")]
public class TreeItem<T>
{
    /// <summary>
    ///     Represents the identifier of the TreeItem in a tree-view
    /// </summary>
    public T? Id { set; get; }

    /// <summary>
    ///     Represents the identifier of the parent of this TreeItem in a tree-view
    /// </summary>
    public T? ParentItemId { set; get; }
}