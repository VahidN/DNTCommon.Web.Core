using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Html TreeView Builder
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class HtmlTreeViewBuilder<TItem>
    where TItem : TreeItem<TItem>
{
    private readonly StringBuilder _htmlBuilder = new();
    private int _numberOfItems;

    /// <summary>
    ///     List of TreeItems
    /// </summary>
    public IEnumerable<TItem>? Items { set; get; }

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
    public Func<TItem, string>? TreeItemBodyTemplate { set; get; }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public (string HtmlDoc, int NumberOfItems) ItemsToHtml()
    {
        if (Items is null || !Items.Any())
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

    private void AppendKids(TItem parentItem)
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

    private List<TItem>? GetKids(TItem parentItem)
        => Items?.Where(treeItem => treeItem.ParentItemId is not null &&
                                    EqualityComparer<TItem>.Default.Equals(treeItem.ParentItemId, parentItem.Id))
            .ToList();

    private void BuildNestedTag(TItem item)
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