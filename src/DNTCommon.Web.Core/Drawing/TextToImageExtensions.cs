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
            SubpixelText = true
        };

        var textBounds = GetTextBounds(text, textPaint);
        var width = GetTextWidth(text, options, textPaint);

        var imageWidth = (int)width + 2 * options.TextMargin;
        var imageHeight = (int)textBounds.Height + 2 * options.TextMargin;

        using var sKBitmap = new SKBitmap(imageWidth, imageHeight);
        using var canvas = new SKCanvas(sKBitmap);
        canvas.Clear(options.BgColor);

        DrawText(text, options, canvas, shaper, textPaint, textBounds);

        if (options.CaptchaNoise is not null)
        {
            AddWaves(imageWidth, imageHeight, sKBitmap);
            CreateNoises(canvas, options);
        }

        DrawRectangle(options, canvas, width, textBounds.Height);

        return ToPng(sKBitmap);
    }

    private static void AddWaves(int width, int height, SKBitmap pic)
    {
        using var copy = new SKBitmap();
        pic.CopyTo(copy);

        double distort = RandomNumberGenerator.GetInt32(1, 6) * (RandomNumberGenerator.GetInt32(1, 3) == 1 ? 1 : -1);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                // Adds a simple wave
                var newX = (int)(x + distort * Math.Sin(Math.PI * y / 84.0));
                var newY = (int)(y + distort * Math.Cos(Math.PI * x / 44.0));

                if (newX < 0 || newX >= width)
                {
                    newX = 0;
                }

                if (newY < 0 || newY >= height)
                {
                    newY = 0;
                }

                pic.SetPixel(x, y, copy.GetPixel(newX, newY));
            }
        }
    }

    private static void CreateNoises(SKCanvas canvas, TextToImageOptions options)
    {
        if (options.CaptchaNoise is null)
        {
            return;
        }

        using var shader = SKShader.CreatePerlinNoiseTurbulence(options.CaptchaNoise.BaseFrequencyX,
            options.CaptchaNoise.BaseFrequencyY, options.CaptchaNoise.NumOctaves, options.CaptchaNoise.Seed);

        using var paint = new SKPaint();
        paint.Shader = shader;
        canvas.DrawPaint(paint);
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

    private static void DrawText(string text,
        TextToImageOptions options,
        SKCanvas canvas,
        SKShaper shaper,
        SKPaint textPaint,
        SKRect textBounds)
    {
        var x = options.TextMargin + textBounds.Left;
        var y = Math.Abs(textBounds.Top) + options.TextMargin;

        canvas.DrawShapedText(shaper, text, x, y, textPaint);

        if (!options.AddDropShadow)
        {
            return;
        }

        textPaint.Color = options.ShadowColor;

        switch (RandomNumberGenerator.GetInt32(1, 5))
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
                StrokeWidth = 1f
            };

            canvas.DrawRect(new SKRect(0, 0, width + 2 * options.TextMargin - 1, height + 2 * options.TextMargin - 1),
                skPaint);
        }
    }

    private static SKTypeface GetFont(TextToImageOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.CustomFontPath))
        {
            return FontsTypeface.GetOrAdd(options.FontName,
                SKTypeface.FromFamilyName(options.FontName, options.FontStyle));
        }

        return FontsTypeface.GetOrAdd(options.CustomFontPath, key =>
        {
            using var embeddedFont = File.OpenRead(key);

            return SKTypeface.FromStream(File.OpenRead(key));
        });
    }

    private static byte[] ToPng(SKBitmap bitmap)
    {
        using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        using var memory = new MemoryStream();
        data.SaveTo(memory);

        return memory.ToArray();
    }
}