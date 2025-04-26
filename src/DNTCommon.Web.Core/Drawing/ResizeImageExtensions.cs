using Microsoft.AspNetCore.Http;
using SkiaSharp;

namespace DNTCommon.Web.Core;

/// <summary>
///     Resizing an image using SkiaSharp
/// </summary>
public static class ResizeImageExtensions
{
    /// <summary>
    ///     Resizing an image using SkiaSharp.
    ///     If you set the scaleFactor, newWidth and newHeight will be ignored.
    /// </summary>
    public static byte[]? ResizeImage(this string filePath, ResizeImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return null;
        }

        using var bitmap = SKBitmap.Decode(filePath);

        return bitmap.ResizeImage(options);
    }

    /// <summary>
    ///     Resizing an image using SkiaSharp
    ///     If you set the scaleFactor, newWidth and newHeight will be ignored.
    /// </summary>
    public static byte[] ResizeImage(this SKBitmap skBitmap, ResizeImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(skBitmap);
        ArgumentNullException.ThrowIfNull(options);

        if (options.ScaleFactor.HasValue)
        {
            (options.NewWidth, options.NewHeight) = GetScaledSize(skBitmap, options.ScaleFactor.Value);
        }

        var skImageInfo = new SKImageInfo(options.NewWidth, options.NewHeight);
        using var scaledBitmap = skBitmap.Resize(skImageInfo, options.FilterQuality);
        using var image = SKImage.FromBitmap(scaledBitmap);
        using var encodedImage = image.Encode(options.Format, options.Quality);
        using var stream = new MemoryStream();
        encodedImage.SaveTo(stream);

        return stream.ToArray();
    }

    /// <summary>
    ///     Resizing an image using SkiaSharp
    ///     If you set the scaleFactor, newWidth and newHeight will be ignored.
    /// </summary>
    public static byte[]? ResizeImage(this byte[]? data, ResizeImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (data is null || data.Length == 0)
        {
            return null;
        }

        using var bitmap = SKBitmap.Decode(data);

        return bitmap.ResizeImage(options);
    }

    /// <summary>
    ///     Resizing an image using SkiaSharp
    ///     If you set the scaleFactor, newWidth and newHeight will be ignored.
    /// </summary>
    public static byte[]? ResizeImage(this Stream? imgStream, ResizeImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (imgStream is null)
        {
            return null;
        }

        using var inputStream = new SKManagedStream(imgStream);
        using var codec = SKCodec.Create(inputStream);
        using var bitmap = SKBitmap.Decode(codec);

        return bitmap.ResizeImage(options);
    }

    /// <summary>
    ///     Resizing an image using SkiaSharp
    ///     If you set the scaleFactor, newWidth and newHeight will be ignored.
    /// </summary>
    public static byte[]? ResizeImage(this IFormFile? fromFile, ResizeImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (fromFile.IsNullOrEmpty())
        {
            return null;
        }

        using var memoryStream = new MemoryStream();
        fromFile.CopyTo(memoryStream);
        memoryStream.Seek(offset: 0, SeekOrigin.Begin);

        using var inputStream = new SKManagedStream(memoryStream);
        using var codec = SKCodec.Create(inputStream);
        using var bitmap = SKBitmap.Decode(codec);

        return bitmap.ResizeImage(options);
    }

    private static (int NewWidth, int NewHeight) GetScaledSize(this SKBitmap skImage, decimal scaleFactor)
    {
        var newWidth = (int)(skImage.Width * scaleFactor);
        var newHeight = (int)(skImage.Height * scaleFactor);

        return (newWidth, newHeight);
    }
}
