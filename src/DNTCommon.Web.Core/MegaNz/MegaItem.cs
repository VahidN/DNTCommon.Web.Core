namespace DNTCommon.Web.Core;

/// <summary>
///     Storage element.
/// </summary>
public class MegaItem
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MegaItem" />.
    /// </summary>
    public MegaItem()
    {
    }

    /// <summary>
    ///     Element name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Parent element.
    /// </summary>
    public MegaItem? Parent { get; set; }

    /// <summary>
    ///     Size in bytes.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    ///     Last time modified.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <inheritdoc />
    public override string ToString() => GetFullPath();

    /// <summary>
    ///     Gets the full path of the element.
    /// </summary>
    /// <returns>Full path.</returns>
    private string GetFullPath()
    {
        var visited = new HashSet<MegaItem>();

        return GetFullPath(visited);
    }

    private string GetFullPath(HashSet<MegaItem> visited)
    {
        if (!visited.Add(this))
        {
            throw new InvalidOperationException(message: "Circular reference detected in parent chain.");
        }

        var path = Name;

        if (path.IsEmpty())
        {
            throw new InvalidOperationException(message: "Entry name is empty.");
        }

        if (Parent is not null)
        {
            path = $"{Parent.GetFullPath(visited)}/{path}";
        }

        return path;
    }
}
