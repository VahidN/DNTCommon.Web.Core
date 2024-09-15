namespace DNTCommon.Web.Core;

/// <summary>
///     A high level utility that converts HTML to PNG.
/// </summary>
public interface IHtmlToPngGenerator
{
    /// <summary>
    ///     High level method that converts HTML to PNG.
    /// </summary>
    Task<string> GeneratePngFromHtmlAsync(HtmlToPngGeneratorOptions options);
}