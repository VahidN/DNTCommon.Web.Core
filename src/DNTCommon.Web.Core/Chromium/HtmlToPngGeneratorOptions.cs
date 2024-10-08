﻿namespace DNTCommon.Web.Core;

/// <summary>
///     HtmlToPngGenerator Options
/// </summary>
public class HtmlToPngGeneratorOptions
{
    /// <summary>
    ///     Source Uri
    /// </summary>
    public string? SourceHtmlFileOrUri { set; get; }

    /// <summary>
    ///     Output path
    /// </summary>
    public string? OutputPngFile { set; get; }

    /// <summary>
    ///     If it's not specified, ChromeFinder.Find with try to find it!
    /// </summary>
    public string? ChromeExecutablePath { get; set; } = "";

    /// <summary>
    ///     Width of the image. Its default value is 800.
    /// </summary>
    public int Width { set; get; } = 800;

    /// <summary>
    ///     Height of the image. Its default value is 600.
    /// </summary>
    public int Height { set; get; } = 600;

    /// <summary>
    ///     Wait for exit. Its default value is 2 minutes.
    /// </summary>
    public TimeSpan WaitForExit { set; get; } = TimeSpan.FromMinutes(value: 3);

    /// <summary>
    ///     ResizeImage Options
    /// </summary>
    public ResizeImageOptions? ResizeImageOptions { set; get; }
}