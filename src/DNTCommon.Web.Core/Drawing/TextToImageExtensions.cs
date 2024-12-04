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
    ///     Draws a text on a bitmap and then returns it as a data:image/png;base64.
    /// </summary>
    public static string TextToBase64DataImage(this string text, TextToImageOptions options)
    {
        var dataBytes = text.TextToImage(options);
        var imageBase64Data = Convert.ToBase64String(dataBytes);

        return $"data:image/png;base64,{imageBase64Data}";
    }

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

        using var textPaint = new SKPaint();
        textPaint.Color = options.FontColor;
        textPaint.IsAntialias = options.AntiAlias;
        textPaint.Style = SKPaintStyle.Fill;

        using var font = new SKFont();
        font.Size = options.FontSize;
        font.Typeface = fontType;
        font.Subpixel = true;

        var textBounds = GetTextBounds(text, font, textPaint);
        var width = GetTextWidth(text, options, fontType);

        var imageWidth = (int)width + 2 * options.TextMargin;
        var imageHeight = (int)textBounds.Height + 2 * options.TextMargin;

        using var sKBitmap = new SKBitmap(imageWidth, imageHeight);
        using var canvas = new SKCanvas(sKBitmap);
        canvas.Clear(options.BgColor);

        DrawText(text, options, canvas, shaper, textPaint, textBounds, SKTextAlign.Left, font);

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

        double distort = RandomNumberGenerator.GetInt32(fromInclusive: 1, toExclusive: 6) *
                         (RandomNumberGenerator.GetInt32(fromInclusive: 1, toExclusive: 3) == 1 ? 1 : -1);

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

    private static float GetTextWidth(string text, TextToImageOptions options, SKTypeface typeface)
    {
        using var blob = typeface.OpenStream().ToHarfBuzzBlob();
        using var hbFace = new Face(blob, index: 0);
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

    private static SKRect GetTextBounds(string text, SKFont font, SKPaint textPaint)
    {
        font.MeasureText(text, out var textBounds, textPaint);

        return textBounds;
    }

    private static void DrawText(string text,
        TextToImageOptions options,
        SKCanvas canvas,
        SKShaper shaper,
        SKPaint textPaint,
        SKRect textBounds,
        SKTextAlign textAlign,
        SKFont font)
    {
        var x = options.TextMargin + textBounds.Left;
        var y = Math.Abs(textBounds.Top) + options.TextMargin;

        canvas.DrawShapedText(shaper, text, x, y, textAlign, font, textPaint);

        if (!options.AddDropShadow)
        {
            return;
        }

        textPaint.Color = options.ShadowColor;

        switch (RandomNumberGenerator.GetInt32(fromInclusive: 1, toExclusive: 5))
        {
            case 1:
                canvas.DrawShapedText(shaper, text, x - 1, y - 1, textAlign, font, textPaint);

                break;

            case 2:
                canvas.DrawShapedText(shaper, text, x + 1, y - 1, textAlign, font, textPaint);

                break;

            case 3:
                canvas.DrawShapedText(shaper, text, x - 1, y + 1, textAlign, font, textPaint);

                break;

            case 4:
                canvas.DrawShapedText(shaper, text, x + 1, y + 1, textAlign, font, textPaint);

                break;
        }
    }

    private static void DrawRectangle(TextToImageOptions options, SKCanvas canvas, float width, float height)
    {
        if (options.Rectangle)
        {
            using var skPaint = new SKPaint();
            skPaint.Color = SKColors.LightGray;
            skPaint.IsStroke = true;
            skPaint.StrokeWidth = 1f;

            canvas.DrawRect(
                new SKRect(left: 0, top: 0, width + 2 * (float)options.TextMargin - 1,
                    height + 2 * (float)options.TextMargin - 1), skPaint);
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
        using var data = bitmap.Encode(SKEncodedImageFormat.Png, quality: 100);
        using var memory = new MemoryStream();
        data.SaveTo(memory);

        return memory.ToArray();
    }
}