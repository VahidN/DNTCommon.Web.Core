using Microsoft.AspNetCore.Http;
using SkiaSharp;

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
    public static bool IsValidImageFile([NotNullWhen(true)] this string? filePath,
        int? maxWidth = null,
        int? maxHeight = null)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return false;
        }

        try
        {
            using var bitmap = SKBitmap.Decode(filePath);

            return bitmap != null && bitmap.Info.HasValidImageInfo(maxWidth, maxHeight);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Tries to decode the given bytes to a bitmap
    /// </summary>
    /// <param name="data">The provided content</param>
    /// <param name="maxWidth">maximum allowed width</param>
    /// <param name="maxHeight">maximum allowed height</param>
    public static bool IsValidImageFile([NotNullWhen(true)] this byte[]? data,
        int? maxWidth = null,
        int? maxHeight = null)
    {
        if (data == null || data.Length == 0)
        {
            return false;
        }

        try
        {
            using var bitmap = SKBitmap.Decode(data);

            return bitmap != null && bitmap.Info.HasValidImageInfo(maxWidth, maxHeight);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Tries to decode the given stream to a bitmap
    /// </summary>
    /// <param name="stream">The stream of a given data</param>
    /// <param name="maxWidth">maximum allowed width</param>
    /// <param name="maxHeight">maximum allowed height</param>
    public static bool IsValidImageFile([NotNullWhen(true)] this Stream? stream,
        int? maxWidth = null,
        int? maxHeight = null)
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

            return bitmap != null && bitmap.Info.HasValidImageInfo(maxWidth, maxHeight);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Tries to decode the given posted file to a bitmap
    /// </summary>
    /// <param name="fromFile">Represents a file sent with the HttpRequest</param>
    /// <param name="maxWidth">maximum allowed width</param>
    /// <param name="maxHeight">maximum allowed height</param>
    public static bool IsValidImageFile([NotNullWhen(true)] this IFormFile? fromFile,
        int? maxWidth = null,
        int? maxHeight = null)
    {
        if (fromFile is null || fromFile.Length == 0)
        {
            return false;
        }

        try
        {
            using var memoryStream = new MemoryStream();
            fromFile.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using var inputStream = new SKManagedStream(memoryStream);
            using var codec = SKCodec.Create(inputStream);
            using var bitmap = SKBitmap.Decode(codec);

            return bitmap != null && bitmap.Info.HasValidImageInfo(maxWidth, maxHeight);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Tries to decode the given posted files to bitmaps
    /// </summary>
    /// <param name="fromFiles">Represents the files sent with the HttpRequest</param>
    /// <param name="maxWidth">maximum allowed width</param>
    /// <param name="maxHeight">maximum allowed height</param>
    public static bool AreValidImageFiles([NotNullWhen(true)] this IFormFileCollection? fromFiles,
        int? maxWidth = null,
        int? maxHeight = null)
    {
        if (fromFiles is null || fromFiles.Count == 0)
        {
            return false;
        }

        return fromFiles.All(fromFile => fromFile.IsValidImageFile(maxWidth, maxHeight));
    }

    private static bool HasValidImageInfo(this SKImageInfo info, int? maxWidth, int? maxHeight)
        => (!maxWidth.HasValue || info.Width <= maxWidth.Value) &&
           (!maxHeight.HasValue || info.Height <= maxHeight.Value);
}