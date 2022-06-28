using System;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Runtime.Versioning;
using SixLabors.ImageSharp.Formats.Png;

namespace DNTCommon.Web.Core;

/// <summary>
/// Text to image extensions
/// </summary>
public static class TextToImageExtensions
{
	private const int Margin = 5;
	
    /// <summary>
    /// Draws a text on a bitmap and then returns it as a png byte array.
    /// </summary>
    public static byte[] TextToImage(this string text, TextToImageOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
		
		var font = getFont(options);
		var (width, height) = getImageSize(text, font);
		using (var image = new Image<Rgba32>(width, height))
		{
			image.Mutate(pc =>
			{
				pc.SetGraphicsOptions(g => g.Antialias = options.AntiAlias);
				drawText(pc, text, options.FontColor, options.BgColor, font);	
				drawRectangle(pc, width, height, options);
			});
			return saveAsPng(image);
		}	
    }
	
	private static void drawText(IImageProcessingContext pc, string text, Color fColor, Color bColor, Font font)
	{
		pc.Fill(bColor);
		pc.DrawText(text, font, fColor, new PointF(Margin, 0));
	}	
	
	private static void drawRectangle(IImageProcessingContext pc, int width, int height, TextToImageOptions options)
	{
		if (options.Rectangle)
		{
			var rectangle = new Rectangle(0, 0, width - 1, height - 1);				
			pc.Draw(options.ShadowColor, 1, rectangle);
		}		
	}	
	
	private static byte[] saveAsPng(Image<Rgba32> image)
	{
		using var stream = new MemoryStream();
		image.Save(stream, new PngEncoder());
		return stream.ToArray();
	}	

	private static (int Width, int Height) getImageSize(string message, Font font)
	{
		var captchaSize = TextMeasurer.Measure(message, new TextOptions(font));
		var width = (int)captchaSize.Width + (2 * Margin);
		var height = (int)captchaSize.Height + Margin;
		return (width, height);
	}
	
	private static Font getFont(TextToImageOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.CustomFontPath))
		{
			var fontFamily = SystemFonts.Get(options.FontName, CultureInfo.InvariantCulture);
			return new Font(fontFamily, options.FontSize);
		}

		var fontCollection = new FontCollection();
		return fontCollection.Add(options.CustomFontPath, CultureInfo.InvariantCulture).CreateFont(options.FontSize);
	}	
}