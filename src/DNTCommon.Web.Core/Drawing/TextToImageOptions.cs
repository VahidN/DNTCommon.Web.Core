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
    ///     Its default value is `SKColors.Black`.
    /// </summary>
    public SKColor FontColor { set; get; } = SKColors.Black;

    /// <summary>
    ///     Its default value is `SKColors.White`.
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
    ///     Adds a random drop shadow
    /// </summary>
    public bool AddDropShadow { set; get; }

    /// <summary>
    ///     Its default value is `SKColors.LightGray`.
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

    /// <summary>
    ///     Defines options of the captcha's noise
    /// </summary>
    public CaptchaNoise? CaptchaNoise { set; get; }
}