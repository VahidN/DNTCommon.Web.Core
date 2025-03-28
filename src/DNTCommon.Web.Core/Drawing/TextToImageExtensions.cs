using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace DNTCommon.Web.Core;

/// <summary>
///     Text to image extensions
/// </summary>
public static class TextToImageExtensions
{
    /// <summary>
    ///     Draws a text on a bitmap and then returns it as a data:image/png;base64.
    /// </summary>
    public static string TextToBase64DataImage(this string text,
        TextToImageOptions options,
        string contentType = "image/png")
    {
        ArgumentNullException.ThrowIfNull(text);

        return text.TextToImage(options).BytesToBase64DataImage(contentType);
    }

    /// <summary>
    ///     returns data as a data:image/png;base64.
    /// </summary>
    public static string BytesToBase64DataImage(this byte[] data, string contentType = "image/png")
    {
        ArgumentNullException.ThrowIfNull(data);

        return $"data:{contentType};base64,{Convert.ToBase64String(data)}";
    }

    /// <summary>
    ///     Draws a rectangle around the given image
    /// </summary>
    public static void DrawFrame(this SKCanvas canvas, SKColor color, float width, float height)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        using var skPaint = new SKPaint();
        skPaint.Color = color;
        skPaint.IsStroke = true;
        skPaint.StrokeWidth = 1f;

        canvas.DrawRect(new SKRect(left: 0, top: 0, width - 1, height - 1), skPaint);
    }

    /// <summary>
    ///     Draws a text at top of the image
    /// </summary>
    public static void DrawTopAlignedText(this SKCanvas canvas,
        int imageWidth,
        string? text,
        ChartFont font,
        SKTextAlign textAlign,
        int margin)
    {
        if (text.IsEmpty())
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(font);

        var titleFontType = font.Name.GetFont(font.Style, font.FilePath);
        using var titleShaper = new SKShaper(titleFontType);

        using var titleFont = new SKFont();
        titleFont.Size = font.Size;
        titleFont.Typeface = titleFontType;

        using var titlePaint = new SKPaint();
        titlePaint.Color = font.Color;
        titlePaint.IsAntialias = true;
        titlePaint.Style = SKPaintStyle.Fill;

        var titleTextBounds = text.GetTextBounds(titleFont, titlePaint);

        var titleX = (float)imageWidth / 2;
        var titleY = (int)titleTextBounds.Height + (2 * margin);

        canvas.DrawShapedText(titleShaper, text, titleX, titleY, textAlign, titleFont, titlePaint);
    }

    /// <summary>
    ///     Draws a text on a bitmap and then returns it as a png byte array.
    /// </summary>
    public static byte[] TextToImage(this string text, TextToImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var fontType = options.FontName.GetFont(options.FontStyle, options.CustomFontPath);
        using var shaper = new SKShaper(fontType);

        using var textPaint = new SKPaint();
        textPaint.Color = options.FontColor;
        textPaint.IsAntialias = options.AntiAlias;
        textPaint.Style = SKPaintStyle.Fill;

        using var font = new SKFont();
        font.Size = options.FontSize;
        font.Typeface = fontType;
        font.Subpixel = true;

        var textBounds = text.GetTextBounds(font, textPaint);
        var width = text.GetTextWidth(options.FontSize, fontType);

        var imageWidth = (int)width + (2 * options.TextMargin);
        var imageHeight = (int)textBounds.Height + (2 * options.TextMargin);

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

        return sKBitmap.ToImageBytes(SKEncodedImageFormat.Png, quality: 100);
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
                var newX = (int)(x + (distort * Math.Sin(Math.PI * y / 84.0)));
                var newY = (int)(y + (distort * Math.Cos(Math.PI * x / 44.0)));

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
                new SKRect(left: 0, top: 0, width + (2 * (float)options.TextMargin) - 1,
                    height + (2 * (float)options.TextMargin) - 1), skPaint);
        }
    }
}
