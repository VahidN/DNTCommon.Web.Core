using System.Collections.Concurrent;
using HarfBuzzSharp;
using SkiaSharp;
using SkiaSharp.HarfBuzz;
using Buffer = HarfBuzzSharp.Buffer;

namespace DNTCommon.Web.Core;

/// <summary>
///     Text to image extensions
/// </summary>
public static class TextToImageExtensions
{
    private static readonly ConcurrentDictionary<string, SKTypeface> FontsTypeface =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Draws a text on a bitmap and then returns it as a png byte array.
    /// </summary>
    public static byte[] TextToImage(this string text, TextToImageOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var fontType = GetFont(options);
        using var shaper = new SKShaper(fontType);
        using var textPaint = new SKPaint
                              {
                                  IsAntialias = options.AntiAlias,
                                  FilterQuality = SKFilterQuality.High,
                                  TextSize = options.FontSize,
                                  Color = options.FontColor,
                                  TextAlign = SKTextAlign.Left,
                                  Typeface = fontType,
                                  SubpixelText = true,
                              };

        var textBounds = GetTextBounds(text, textPaint);
        var width = GetTextWidth(text, options, textPaint);

        var info = new SKImageInfo((int)width + 2 * options.TextMargin,
                                   (int)textBounds.Height + 2 * options.TextMargin);
        using var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;
        canvas.Clear(options.BgColor);

        DrawRectangle(options, canvas, width, textBounds.Height);
        DrawText(text, options, canvas, shaper, textPaint, textBounds);
        return ToPng(surface);
    }

    private static float GetTextWidth(string text, TextToImageOptions options, SKPaint textPaint)
    {
        using var blob = textPaint.Typeface.OpenStream().ToHarfBuzzBlob();
        using var hbFace = new Face(blob, 0);
        using var hbFont = new Font(hbFace);
        using var buffer = new Buffer();
        buffer.AddUtf16(text);
        buffer.GuessSegmentProperties();
        hbFont.Shape(buffer);

        hbFont.GetScale(out var xScale, out _);
        var scale = options.FontSize / (float)xScale;
        var width = buffer.GlyphPositions.Sum(position => position.XAdvance) * scale;
        return width;
    }

    private static SKRect GetTextBounds(string text, SKPaint textPaint)
    {
        var textBounds = new SKRect();
        textPaint.MeasureText(text, ref textBounds);
        return textBounds;
    }

    private static void DrawText(string text, TextToImageOptions options, SKCanvas canvas, SKShaper shaper,
                                 SKPaint textPaint, SKRect textBounds)
    {
        var x = options.TextMargin + textBounds.Left;
        var y = Math.Abs(textBounds.Top) + options.TextMargin;

        canvas.DrawShapedText(shaper, text, x, y, textPaint);

        if (options.DropShadowLevel <= 0)
        {
            return;
        }

        textPaint.Color = options.ShadowColor;

        switch (options.DropShadowLevel)
        {
            case 1:
                canvas.DrawShapedText(shaper, text, x - 1, y - 1, textPaint);
                break;

            case 2:
                canvas.DrawShapedText(shaper, text, x + 1, y - 1, textPaint);
                break;

            case 3:
                canvas.DrawShapedText(shaper, text, x - 1, y + 1, textPaint);
                break;

            case 4:
                canvas.DrawShapedText(shaper, text, x + 1, y + 1, textPaint);
                break;
        }
    }

    private static void DrawRectangle(TextToImageOptions options, SKCanvas canvas, float width, float height)
    {
        if (options.Rectangle)
        {
            using var skPaint = new SKPaint
                                {
                                    Color = SKColors.LightGray,
                                    IsStroke = true,
                                    StrokeWidth = 1f,
                                };
            canvas.DrawRect(new SKRect(0, 0, width + 2 * options.TextMargin - 1,
                                       height + 2 * options.TextMargin - 1), skPaint);
        }
    }

    private static SKTypeface GetFont(TextToImageOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.CustomFontPath))
        {
            return FontsTypeface.GetOrAdd(options.FontName,
                                          _ => SKTypeface.FromFamilyName(options.FontName, options.FontStyle));
        }

        return FontsTypeface.GetOrAdd(options.CustomFontPath,
                                      _ =>
                                      {
                                          using var embeddedFont = File.OpenRead(options.CustomFontPath);
                                          return SKTypeface.FromStream(File.OpenRead(options.CustomFontPath));
                                      });
    }

    private static byte[] ToPng(this SKSurface surface)
    {
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var memory = new MemoryStream();
        data.SaveTo(memory);
        return memory.ToArray();
    }
}