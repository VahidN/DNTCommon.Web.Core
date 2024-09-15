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
}