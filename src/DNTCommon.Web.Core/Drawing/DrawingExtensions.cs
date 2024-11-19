using SkiaSharp;

namespace DNTCommon.Web.Core;

/// <summary>
///     Some useful drawing methods
/// </summary>
public static class DrawingExtensions
{
    /// <summary>
    ///     Check if image is plain white?
    /// </summary>
    public static bool IsBlankImage(this string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return true;
        }

        using var bitmap = SKBitmap.Decode(filePath);

        return IsBlankImage(bitmap);
    }

    /// <summary>
    ///     Check if image is plain white?
    /// </summary>
    public static bool IsBlankImage(this byte[]? data)
    {
        if (data is null || data.Length == 0)
        {
            return true;
        }

        using var bitmap = SKBitmap.Decode(data);

        return IsBlankImage(bitmap);
    }

    /// <summary>
    ///     Check if image is plain white?
    /// </summary>
    public static bool IsBlankImage(this SKBitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        for (var x = 0; x < bitmap.Width; x++)
        {
            for (var y = 0; y < bitmap.Height; y++)
            {
                var clr = bitmap.GetPixel(x, y);

                if (clr.Red != 255 || clr.Green != 255 || clr.Blue != 255)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    ///     Checks if image is partially white?
    /// </summary>
    public static bool IsPartiallyBlankImage(this string? filePath, int whitePixelsPercentage)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return true;
        }

        using var bitmap = SKBitmap.Decode(filePath);

        return IsPartiallyBlankImage(bitmap, whitePixelsPercentage);
    }

    /// <summary>
    ///     Checks if image is partially white?
    /// </summary>
    public static bool IsPartiallyBlankImage(this byte[]? data, int whitePixelsPercentage)
    {
        if (data is null || data.Length == 0)
        {
            return true;
        }

        using var bitmap = SKBitmap.Decode(data);

        return IsPartiallyBlankImage(bitmap, whitePixelsPercentage);
    }

    /// <summary>
    ///     Checks if image is partially white?
    /// </summary>
    public static bool IsPartiallyBlankImage(this SKBitmap bitmap, int whitePixelsPercentage)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        long whitePixels = 0;
        long totalPixels = bitmap.Width * bitmap.Height;

        for (var x = 0; x < bitmap.Width; x++)
        {
            for (var y = 0; y < bitmap.Height; y++)
            {
                var clr = bitmap.GetPixel(x, y);

                if (clr is { Red: 255, Green: 255, Blue: 255 })
                {
                    whitePixels++;

                    var blankPixelsPercentage = whitePixels / (float)totalPixels * 100;

                    if (blankPixelsPercentage >= whitePixelsPercentage)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    ///     Determines how much of this image is white.
    /// </summary>
    public static float? GetImageBlankPixelsPercentage(this string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return null;
        }

        using var bitmap = SKBitmap.Decode(filePath);

        return GetImageBlankPixelsPercentage(bitmap);
    }

    /// <summary>
    ///     Determines how much of this image is white.
    /// </summary>
    public static float? GetImageBlankPixelsPercentage(this byte[]? data)
    {
        if (data is null || data.Length == 0)
        {
            return null;
        }

        using var bitmap = SKBitmap.Decode(data);

        return GetImageBlankPixelsPercentage(bitmap);
    }

    /// <summary>
    ///     Determines how much of this image is white.
    /// </summary>
    public static float GetImageBlankPixelsPercentage(this SKBitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        long whitePixels = 0;
        long totalPixels = bitmap.Width * bitmap.Height;

        for (var x = 0; x < bitmap.Width; x++)
        {
            for (var y = 0; y < bitmap.Height; y++)
            {
                var clr = bitmap.GetPixel(x, y);

                if (clr is { Red: 255, Green: 255, Blue: 255 })
                {
                    whitePixels++;
                }
            }
        }

        return whitePixels / (float)totalPixels * 100;
    }
}