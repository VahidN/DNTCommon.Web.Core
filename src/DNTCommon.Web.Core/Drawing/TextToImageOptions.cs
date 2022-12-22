using SkiaSharp;

namespace DNTCommon.Web.Core;

/// <summary>
///     Text To Image Options
/// </summary>
public class TextToImageOptions
{
    /// <summary>
    ///     Its default value is `verdana`.
    /// </summary>
    public string FontName { set; get; } = "Tahoma";

    /// <summary>
    ///     You can specify an optional custom local font here. In this case, the `FontName` will be ignored.
    /// </summary>
    public string? CustomFontPath { set; get; }

    /// <summary>
    ///     Its default value is `13`.
    /// </summary>
    public int FontSize { set; get; } = 13;

    /// <summary>
    ///     Its default value is `Color.Black`.
    /// </summary>
    public SKColor FontColor { set; get; } = SKColors.Black;

    /// <summary>
    ///     Its default value is `Color.White`.
    /// </summary>
    public SKColor BgColor { set; get; } = SKColors.White;

    /// <summary>
    ///     Its default value is `5`.
    /// </summary>
    public int TextMargin { set; get; } = 5;

    /// <summary>
    ///     Its default value is `SKFontStyle.Normal`.
    /// </summary>
    public SKFontStyle FontStyle { set; get; } = SKFontStyle.Normal;

    /// <summary>
    ///     It's a number between 1 to 4.
    ///     Its default value is `0`.
    ///     Set it to zero to disable it.
    /// </summary>
    public int DropShadowLevel { set; get; }

    /// <summary>
    ///     Its default value is `Color.LightGray`.
    /// </summary>
    public SKColor ShadowColor { set; get; } = SKColors.LightGray;

    /// <summary>
    ///     Its default value is `true`.
    /// </summary>
    public bool AntiAlias { set; get; } = true;

    /// <summary>
    ///     Its default value is `true`.
    /// </summary>
    public bool Rectangle { set; get; } = true;
}