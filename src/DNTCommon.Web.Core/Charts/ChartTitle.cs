using SkiaSharp;

namespace DNTCommon.Web.Core;

/// <summary>
///     Chart's title info
/// </summary>
public class ChartTitle
{
    /// <summary>
    ///     Its default value is `5`.
    /// </summary>
    public int Margin { set; get; } = 5;

    /// <summary>
    ///     The chart's title
    /// </summary>
    public string? Text { set; get; }

    /// <summary>
    ///     The title's font info
    /// </summary>
    public ChartFont Font { set; get; } = new();

    /// <summary>
    ///     Its default value is SKTextAlign.Center
    /// </summary>
    public SKTextAlign TextAlign { set; get; } = SKTextAlign.Center;
}
