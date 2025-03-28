using SkiaSharp;

namespace DNTCommon.Web.Core;

/// <summary>
///     Chart's font info
/// </summary>
public class ChartFont
{
    /// <summary>
    ///     The font's name. It's an optional value. If it's not specified, the CustomFontPath will be used.
    /// </summary>
    public string? Name { set; get; }

    /// <summary>
    ///     You can specify an optional custom local font here. In this case, the `FontName` will be ignored.
    /// </summary>
    public string? FilePath { set; get; }

    /// <summary>
    ///     Its default value is `14`.
    /// </summary>
    public int Size { set; get; } = 14;

    /// <summary>
    ///     Its default value is `SKFontStyle.Normal`.
    /// </summary>
    public SKFontStyle Style { set; get; } = SKFontStyle.Normal;

    /// <summary>
    ///     Its default value is `SKColors.Black`.
    /// </summary>
    public SKColor Color { set; get; } = SKColors.Black;
}
