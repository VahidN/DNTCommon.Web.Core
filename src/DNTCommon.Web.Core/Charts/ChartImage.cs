using SkiaSharp;

namespace DNTCommon.Web.Core;

/// <summary>
///     Chart's image info
/// </summary>
public class ChartImage
{
    /// <summary>
    ///     Its default value is 600.
    /// </summary>
    public int Height { get; set; } = 600;

    /// <summary>
    ///     Its default value is 600.
    /// </summary>
    public int Width { get; set; } = 600;

    /// <summary>
    ///     The various formats used by a SKCodec.
    /// </summary>
    public SKEncodedImageFormat Format { set; get; } = SKEncodedImageFormat.Png;

    /// <summary>
    ///     Quality of the  image
    /// </summary>
    public int Quality { set; get; } = 100;

    /// <summary>
    ///     Its default value is `white`.
    /// </summary>
    public SKColor? BackgroundColor { get; set; }

    /// <summary>
    ///     Should we display a frame for the produced image?
    /// </summary>
    public bool ShowFrame { set; get; }

    /// <summary>
    ///     Its default value is `SKColors.LightGray`.
    /// </summary>
    public SKColor FrameColor { get; set; } = SKColors.LightGray;
}
