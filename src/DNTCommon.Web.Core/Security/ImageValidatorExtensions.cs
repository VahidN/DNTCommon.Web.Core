using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using Exception = System.Exception;

namespace DNTCommon.Web.Core;

/// <summary>
///     Tries to decode the given data to a bitmap to validate its content
/// </summary>
public static class ImageValidatorExtensions
{
    /// <summary>
    ///     Tries to decode the given file to a bitmap
    /// </summary>
    /// <param name="filePath">The absolute path of the file</param>
    /// <param name="maxWidth">maximum allowed width</param>
    /// <param name="maxHeight">maximum allowed height</param>
    /// <param name="logger">An optional error's logger</param>
    public static bool IsValidImageFile([NotNullWhen(returnValue: true)] this string? filePath,
        int? maxWidth = null,
        int? maxHeight = null,
        ILogger? logger = null)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return false;
        }

        try
        {
            using var bitmap = SKBitmap.Decode(filePath);

            return bitmap?.Info.HasValidImageInfo(maxWidth, maxHeight) == true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "`{File}` is an invalid image file.", filePath);

            return false;
        }
    }

    /// <summary>
    ///     Tries to decode the given bytes to a bitmap
    /// </summary>
    /// <param name="data">The provided content</param>
    /// <param name="maxWidth">maximum allowed width</param>
    /// <param name="maxHeight">maximum allowed height</param>
    /// <param name="logger">An optional error's logger</param>
    public static bool IsValidImageFile([NotNullWhen(returnValue: true)] this byte[]? data,
        int? maxWidth = null,
        int? maxHeight = null,
        ILogger? logger = null)
    {
        if (data is null || data.Length == 0)
        {
            return false;
        }

        try
        {
            using var bitmap = SKBitmap.Decode(data);

            return bitmap?.Info.HasValidImageInfo(maxWidth, maxHeight) == true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "This is an invalid image file.");

            return false;
        }
    }

    /// <summary>
    ///     Tries to decode the given stream to a bitmap
    /// </summary>
    /// <param name="stream">The stream of a given data</param>
    /// <param name="maxWidth">maximum allowed width</param>
    /// <param name="maxHeight">maximum allowed height</param>
    /// <param name="logger">An optional error's logger</param>
    public static bool IsValidImageFile([NotNullWhen(returnValue: true)] this Stream? stream,
        int? maxWidth = null,
        int? maxHeight = null,
        ILogger? logger = null)
    {
        if (stream is null)
        {
            return false;
        }

        try
        {
            using var inputStream = new SKManagedStream(stream);
            using var codec = SKCodec.Create(inputStream);
            using var bitmap = SKBitmap.Decode(codec);

            return bitmap?.Info.HasValidImageInfo(maxWidth, maxHeight) == true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "This is an invalid image file.");

            return false;
        }
    }

    /// <summary>
    ///     Tries to decode the given posted file to a bitmap
    /// </summary>
    /// <param name="fromFile">Represents a file sent with the HttpRequest</param>
    /// <param name="maxWidth">maximum allowed width</param>
    /// <param name="maxHeight">maximum allowed height</param>
    /// <param name="logger">An optional error's logger</param>
    public static bool IsValidImageFile([NotNullWhen(returnValue: true)] this IFormFile? fromFile,
        int? maxWidth = null,
        int? maxHeight = null,
        ILogger? logger = null)
    {
        if (fromFile.IsNullOrEmpty())
        {
            return false;
        }

        try
        {
            using var memoryStream = new MemoryStream();
            fromFile.CopyTo(memoryStream);
            memoryStream.Seek(offset: 0, SeekOrigin.Begin);

            using var inputStream = new SKManagedStream(memoryStream);
            using var codec = SKCodec.Create(inputStream);
            using var bitmap = SKBitmap.Decode(codec);

            return bitmap?.Info.HasValidImageInfo(maxWidth, maxHeight) == true;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "This is an invalid image file.");

            return false;
        }
    }

    /// <summary>
    ///     Tries to decode the given posted files to bitmaps
    /// </summary>
    /// <param name="fromFiles">Represents the files sent with the HttpRequest</param>
    /// <param name="maxWidth">maximum allowed width</param>
    /// <param name="maxHeight">maximum allowed height</param>
    /// <param name="logger">An optional error's logger</param>
    public static bool AreValidImageFiles([NotNullWhen(returnValue: true)] this IFormFileCollection? fromFiles,
        int? maxWidth = null,
        int? maxHeight = null,
        ILogger? logger = null)
        => fromFiles is not null && fromFiles.Count != 0 &&
           fromFiles.All(fromFile => fromFile.IsValidImageFile(maxWidth, maxHeight, logger));

    /// <summary>
    ///     Does this image has a proper Width and Height
    /// </summary>
    public static bool HasValidImageInfo(this SKImageInfo info, int? maxWidth = null, int? maxHeight = null)
        => (maxWidth is null && maxHeight is null) || (maxWidth.HasValue && info.Width <= maxWidth.Value) ||
           (maxHeight.HasValue && info.Height <= maxHeight.Value);
}
