using HarfBuzzSharp;
using SkiaSharp;
using SkiaSharp.HarfBuzz;
using Buffer = HarfBuzzSharp.Buffer;

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

    /// <summary>
    ///     Encodes the given bitmap using the specified format and returns its data as a byte array
    /// </summary>
    public static byte[] ToImageBytes(this SKBitmap bitmap, SKEncodedImageFormat format, int quality)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        using var image = SKImage.FromBitmap(bitmap);
        using var encodedImage = image.Encode(format, quality);

        using var stream = new MemoryStream();
        encodedImage.SaveTo(stream);

        return stream.ToArray();
    }

    /// <summary>
    ///     Calculates the given text's width based on the provided font's info.
    /// </summary>
    public static float GetTextWidth(this string? text, SKFont font)
    {
        if (text.IsEmpty()) { return 0; }

        ArgumentNullException.ThrowIfNull(font);

        return text.GetTextWidth(font.Size, font.Typeface);
    }

    /// <summary>
    ///     Calculates the given text's width based on the provided font's info.
    /// </summary>
    public static float GetTextWidth(this string? text, float fontSize, SKTypeface typeface)
    {
        if (text.IsEmpty()) { return 0; }

        ArgumentNullException.ThrowIfNull(typeface);

        using var streamAsset = typeface.OpenStream();
        using var blob = streamAsset.ToHarfBuzzBlob();
        using var hbFace = new Face(blob, index: 0);
        using var hbFont = new Font(hbFace);
        using var buffer = new Buffer();
        buffer.AddUtf16(text);
        buffer.GuessSegmentProperties();
        hbFont.Shape(buffer);

        hbFont.GetScale(out var xScale, out _);
        var scale = fontSize / xScale;
        var width = buffer.GlyphPositions.Sum(position => position.XAdvance) * scale;

        return width;
    }

    /// <summary>
    ///     Measures the given text's bounds
    /// </summary>
    public static SKRect GetTextBounds(this string text, SKFont font, SKPaint? textPaint = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(font);

        font.MeasureText(text, out var textBounds, textPaint);

        return textBounds;
    }
}
