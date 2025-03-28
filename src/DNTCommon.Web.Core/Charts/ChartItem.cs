using SkiaSharp;

namespace DNTCommon.Web.Core;

/// <summary>
///     Defines a Chart Item
/// </summary>
public class ChartItem
{
    /// <summary>
    ///     Defines a Chart Item
    /// </summary>
    public ChartItem()
    {
    }

    /// <summary>
    ///     Defines a Chart Item
    /// </summary>
    public ChartItem(string label, float value, SKColor color)
    {
        Label = label;
        Value = value;
        Color = color;
    }

    /// <summary>
    ///     Defines the label of the item
    /// </summary>
    public string? Label { set; get; }

    /// <summary>
    ///     Defines the value of the item
    /// </summary>
    public float Value { set; get; }

    /// <summary>
    ///     Defines the color of the item
    /// </summary>
    public SKColor? Color { set; get; }
}
