using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.Versioning;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Text to image extensions
    /// </summary>
    public static class TextToImageExtensions
    {
        /// <summary>
        /// Measures the size of the text according to the given font.
        /// </summary>
#if !NETCORE3_1
        [SupportedOSPlatform("windows")]
#endif
        public static SizeF MeasureString(this string text, Font font)
        {
            using var bmp = new Bitmap(1, 1);
            using var graphics = Graphics.FromImage(bmp);
            return graphics.MeasureString(text, font);
        }

        /// <summary>
        /// Draws a text on a bitmap and then returns it as a png byte array.
        /// </summary>
#if !NETCORE3_1
        [SupportedOSPlatform("windows")]
#endif
        public static byte[] TextToImage(this string text, TextToImageOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            using var family = new FontFamily(options.FontName);
            using var font = new Font(family, options.FontSize, options.FontStyle, GraphicsUnit.Pixel);
            var textSize = MeasureString(text, font);
            int width = ((int)textSize.Width) + 5;
            int height = ((int)textSize.Height) + 3;

            var rectangle = new RectangleF(0, 0, width, height);
            using var pic = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(pic);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.High;
            if (options.AntiAlias) graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            using var fgBrush = new SolidBrush(options.FontColor);
            using var bgBrush = new SolidBrush(options.BgColor);
            if (options.Rectangle)
            {
                graphics.FillRectangle(bgBrush, rectangle);
            }
            else
            {
                using var brush = new SolidBrush(Color.White);
                graphics.FillRectangle(brush, rectangle);
                graphics.FillEllipse(bgBrush, rectangle);
            }

            using var pen = new Pen(Color.LightGray);
            graphics.DrawRectangle(pen, new Rectangle(0, 0, width - 1, height - 1));
            drawString(text, options, font, rectangle, graphics, fgBrush);

            using var memory = new MemoryStream();
            pic.Save(memory, ImageFormat.Png);
            return memory.ToArray();
        }

#if !NETCORE3_1
        [SupportedOSPlatform("windows")]
#endif
        private static void drawString(
                    string text,
                    TextToImageOptions options,
                    Font font,
                    RectangleF rectangle,
                    Graphics graphics,
                    SolidBrush fgBrush)
        {
            using var format = new StringFormat
            {
                FormatFlags = StringFormatFlags.NoWrap,
                Alignment = StringAlignment.Center
            };

            if (options.DropShadowLevel > 0)
            {
                using var brush = new SolidBrush(options.ShadowColor);
                switch (options.DropShadowLevel)
                {
                    case 1:
                        rectangle.Offset(-1, -1);
                        graphics.DrawString(text, font, brush, rectangle, format);
                        rectangle.Offset(+1, +1);
                        break;

                    case 2:
                        rectangle.Offset(+1, -1);
                        graphics.DrawString(text, font, brush, rectangle, format);
                        rectangle.Offset(-1, +1);
                        break;

                    case 3:
                        rectangle.Offset(-1, +1);
                        graphics.DrawString(text, font, brush, rectangle, format);
                        rectangle.Offset(+1, -1);
                        break;

                    case 4:
                        rectangle.Offset(+1, +1);
                        graphics.DrawString(text, font, brush, rectangle, format);
                        rectangle.Offset(-1, -1);
                        break;
                }
            }

            graphics.DrawString(text, font, fgBrush, rectangle, format);
        }
    }
}