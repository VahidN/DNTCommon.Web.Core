using SkiaSharp;

namespace DNTCommon.Web.Core;

/// <summary>
///     ResizeImage Options
/// </summary>
public class ResizeImageOptions
{
    /// <summary>
    ///     If you set the scaleFactor, newWidth will be ignored.
    /// </summary>
    public int NewWidth { set; get; }

    /// <summary>
    ///     If you set the scaleFactor, newHeight will be ignored.
    /// </summary>
    public int NewHeight { set; get; }

    /// <summary>
    ///     The new ScaleFactor of the resized image
    /// </summary>
    public decimal? ScaleFactor { set; get; }

    /// <summary>
    ///     The various formats used by a SKCodec.
    /// </summary>
    public SKEncodedImageFormat Format { set; get; } = SKEncodedImageFormat.Jpeg;

    /// <summary>
    ///     Filter quality settings.
    /// </summary>
    public SKSamplingOptions FilterQuality { set; get; } = new(SKFilterMode.Linear, SKMipmapMode.Linear);

    /// <summary>
    ///     Quality of the resized image
    /// </summary>
    public int Quality { set; get; } = 90;
}