using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Html TreeView Builder
/// </summary>
/// <typeparam name="T"></typeparam>
public class HtmlTreeViewBuilder<T>
{
    private readonly StringBuilder _htmlBuilder = new();
    private int _numberOfItems;

    /// <summary>
    /// </summary>
    public ICollection<TreeItem<T>>? Items { set; get; }

    /// <summary>
    ///     Its default value is &lt;ul&gt;
    /// </summary>
    public string? UnOrderedListStartTag { set; get; } = "<ul>";

    /// <summary>
    ///     Its default value is &lt;/ul&gt;
    /// </summary>
    public string? UnOrderedListEndTag { set; get; } = "</ul>";

    /// <summary>
    ///     Its default value is &lt;li&gt;
    /// </summary>
    public string? ListItemStartTag { set; get; } = "<li>";

    /// <summary>
    ///     Its default value is &lt;/li&gt;
    /// </summary>
    public string? ListItemEndTag { set; get; } = "</li>";

    /// <summary>
    ///     Dynamic template of the outer div
    /// </summary>
    public Func<string, string>? OuterDivTemplate { set; get; }

    /// <summary>
    ///     Dynamic template of an item
    /// </summary>
    public Func<TreeItem<T>, string>? TreeItemBodyTemplate { set; get; }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public (string HtmlDoc, int NumberOfItems) ItemsToHtml()
    {
        if (Items is null || Items.Count == 0)
        {
            return (string.Empty, 0);
        }

        if (OuterDivTemplate is null)
        {
            throw new InvalidOperationException($"{nameof(OuterDivTemplate)} is null.");
        }

        _htmlBuilder.Append(UnOrderedListStartTag);

        foreach (var item in Items.Where(treeItem => treeItem.ParentItemId is null))
        {
            BuildNestedTag(item);
        }

        _htmlBuilder.Append(UnOrderedListEndTag);

        return (OuterDivTemplate(_htmlBuilder.ToString()), _numberOfItems);
    }

    private void AppendKids(TreeItem<T> parentItem)
    {
        var kids = GetKids(parentItem);

        if (kids is null || kids.Count == 0)
        {
            return;
        }

        _htmlBuilder.Append(UnOrderedListStartTag);

        foreach (var kid in kids)
        {
            BuildNestedTag(kid);
        }

        _htmlBuilder.Append(UnOrderedListEndTag);
    }

    private List<TreeItem<T>>? GetKids(TreeItem<T> parentItem)
        => Items?.Where(treeItem => treeItem.ParentItemId is not null &&
                                    EqualityComparer<T>.Default.Equals(treeItem.ParentItemId, parentItem.Id))
            .ToList();

    private void BuildNestedTag(TreeItem<T> item)
    {
        if (TreeItemBodyTemplate is null)
        {
            throw new InvalidOperationException($"{nameof(TreeItemBodyTemplate)} is null.");
        }

        _htmlBuilder.Append(ListItemStartTag);
        _htmlBuilder.Append(TreeItemBodyTemplate(item));

        _numberOfItems++;
        AppendKids(item);
        _htmlBuilder.Append(ListItemEndTag);
    }
}