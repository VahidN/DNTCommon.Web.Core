namespace DNTCommon.Web.Core;

/// <summary>
///     FixRemoteImages Options
/// </summary>
public class FixRemoteImagesOptions
{
    /// <summary>
    ///     Maximum allowed width
    /// </summary>
    public int MaxWidth { set; get; } = 800;

    /// <summary>
    ///     Maximum allowed height
    /// </summary>
    public int MaxHeight { set; get; } = 800;

    /// <summary>
    ///     Resize ScaleFactor
    /// </summary>
    public ResizeImageOptions ResizeImageOptions { set; get; } = new()
    {
        ScaleFactor = 0.7M
    };

    /// <summary>
    ///     The output image folder path
    /// </summary>
    public string? OutputImageFolder { set; get; }

    /// <summary>
    ///     The current Request's root address
    /// </summary>
    public Uri? HostUri { set; get; }

    /// <summary>
    ///     This delegate gives you HostUri and SavedFileName and then expects the final image url
    /// </summary>
    public Func<string, string>? ImageUrlBuilder { set; get; }
}