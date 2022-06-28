using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace DNTCommon.Web.Core;

/// <summary>
/// Text To Image Options
/// </summary>
public class TextToImageOptions
{
    /// <summary>
    /// Its default value is `verdana`.
    /// </summary>
    public string FontName { set; get; } = "verdana";	

    /// <summary>
    /// You can specify an optional custom local font here. In this case, the `FontName` will be ignored.
    /// </summary>	
	public string? CustomFontPath { set; get; }

    /// <summary>
    /// Its default value is `13`.
    /// </summary>
    public int FontSize { set; get; } = 13;

    /// <summary>
    /// Its default value is `Color.Black`.
    /// </summary>
    public Color FontColor { set; get; } = Color.Black;

    /// <summary>
    /// Its default value is `Color.White`.
    /// </summary>
    public Color BgColor { set; get; } = Color.White;

    /// <summary>
    /// Its default value is `FontStyle.Regular`.
    /// </summary>
    public FontStyle FontStyle { set; get; }

    /// <summary>
    /// Its default value is `3`.
    /// </summary>
    public int DropShadowLevel { set; get; } = 3;

    /// <summary>
    /// Its default value is `Color.LightGray`.
    /// </summary>
    public Color ShadowColor { set; get; } = Color.LightGray;

    /// <summary>
    /// Its default value is `true`.
    /// </summary>
    public bool AntiAlias { set; get; } = true;

    /// <summary>
    /// Its default value is `true`.
    /// </summary>
    public bool Rectangle { set; get; } = true;
}