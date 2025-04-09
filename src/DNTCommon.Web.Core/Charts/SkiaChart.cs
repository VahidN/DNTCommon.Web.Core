using SkiaSharp;

namespace DNTCommon.Web.Core;

/// <summary>
///     Defines a template for out custom charts
/// </summary>
public abstract class SkiaChart
{
    /// <summary>
    ///     Chart's items to be displayed. It's a collection of labels and their corresponding values.
    /// </summary>
    public IReadOnlyList<ChartItem> Items { set; get; } = new List<ChartItem>();

    /// <summary>
    ///     Chart's image info
    /// </summary>
    public ChartImage Image { set; get; } = new();

    /// <summary>
    ///     The labels font info
    /// </summary>
    public ChartFont LabelsFont { set; get; } = new();

    /// <summary>
    ///     Determines whether LabelFont's color should be used for drawing labels or item colors
    /// </summary>
    public bool UseItemColorsForLabels { set; get; }

    /// <summary>
    ///     Its default value is SKTextAlign.Center
    /// </summary>
    public SKTextAlign LabelsTextAlign { set; get; } = SKTextAlign.Center;

    /// <summary>
    ///     Defines the chart's title info
    /// </summary>
    public ChartTitle? Title { set; get; }

    /// <summary>
    ///     The Percent Format. Its default value is `F1`.
    /// </summary>
    public string NumbersFormat { set; get; } = "F1";

    /// <summary>
    ///     Draws a chart and returns it as a .png byte array by default
    /// </summary>
    public abstract byte[] Draw();

    /// <summary>
    ///     Draws a chart and returns it as a .png stream by default
    /// </summary>
    public abstract Stream DrawAsStream();

    protected string GetFormattedNumber(float number) => number.ToString(NumbersFormat, CultureInfo.InvariantCulture);
}
