namespace DNTCommon.Web.Core;

/// <summary>
///     HtmlToPngGenerator Options
/// </summary>
public class HtmlToPngGeneratorOptions : BaseChromiumGeneratorOptions
{
    /// <summary>
    ///     Width of the image. Its default value is 800.
    /// </summary>
    public int Width { set; get; } = 800;

    /// <summary>
    ///     Height of the image. Its default value is 600.
    /// </summary>
    public int Height { set; get; } = 600;

    /// <summary>
    ///     ResizeImage Options
    /// </summary>
    public ResizeImageOptions? ResizeImageOptions { set; get; }

    /// <inheritdoc />
    public override string ToString()
        => $"SourceHtmlFileOrUri:`{SourceHtmlFileOrUri}`, OutputPngFile:`{OutputFilePath}`, Width:`{Width}`, Height:`{Height}`, WaitForExit:`{WaitForExit}`.";
}