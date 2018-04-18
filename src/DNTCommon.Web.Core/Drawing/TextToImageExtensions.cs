using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Text To Image Options
    /// </summary>
    public class TextToImageOptions
    {
        /// <summary>
        /// Its default value is `verdana`.
        /// </summary>
        public string FontName { set; get; } = "verdana";

        /// <summary>
        /// Its default value is `13`.
        /// </summary>
        public int FontSize { set; get; } = 13;

        /// <summary>
        /// Its default value is `Color.Black`.
        /// </summary>
        public Color FontColor { set; get; } = Color.Black;

        /// <summary>
        /// Its default value is `Color.White`.
        /// </summary>
        public Color BgColor { set; get; } = Color.White;

        /// <summary>
        /// Its default value is `FontStyle.Regular`.
        /// </summary>
        public FontStyle FontStyle { set; get; } = FontStyle.Regular;

        /// <summary>
        /// Its default value is `3`.
        /// </summary>
        public int DropShadowLevel { set; get; } = 3;

        /// <summary>
        /// Its default value is `Color.LightGray`.
        /// </summary>
        public Color ShadowColor { set; get; } = Color.LightGray;

        /// <summary>
        /// Its default value is `true`.
        /// </summary>
        public bool AntiAlias { set; get; } = true;

        /// <summary>
        /// Its default value is `true`.
        /// </summary>
        public bool Rectangle { set; get; } = true;
    }

    /// <summary>
    /// Text to image extensions
    /// </summary>
    public static class TextToImageExtensions
    {
        /// <summary>
        /// Measures the size of the text according to the given font.
        /// </summary>
        public static SizeF MeasureString(this string text, Font font)
        {
            using (var bmp = new Bitmap(1, 1))
            {
                using (var graphics = Graphics.FromImage(bmp))
                {
                    return graphics.MeasureString(text, font);
                }
            }
        }

        /// <summary>
        /// Draws a text on a bitmap and then returns it as a png byte array.
        /// </summary>
        public static byte[] TextToImage(this string text, TextToImageOptions options)
        {
            using (Font font = new Font(new FontFamily(options.FontName), options.FontSize, options.FontStyle, GraphicsUnit.Pixel))
            {
                var textSize = MeasureString(text, font);
                int width = ((int)textSize.Width) + 5;
                int height = ((int)textSize.Height) + 3;

                var rectangle = new RectangleF(0, 0, width, height);
                using (Bitmap pic = new Bitmap(width, height))
                {
                    using (Graphics graphics = Graphics.FromImage(pic))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.High;
                        if (options.AntiAlias) graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                        using (SolidBrush fgBrush = new SolidBrush(options.FontColor))
                        {
                            using (SolidBrush bgBrush = new SolidBrush(options.BgColor))
                            {
                                if (options.Rectangle)
                                {
                                    graphics.FillRectangle(bgBrush, rectangle);
                                }
                                else
                                {
                                    graphics.FillRectangle(new SolidBrush(Color.White), rectangle);
                                    graphics.FillEllipse(bgBrush, rectangle);
                                }

                                graphics.DrawRectangle(new Pen(Color.LightGray), new Rectangle(0, 0, width - 1, height - 1));

                                using (StringFormat format = new StringFormat())
                                {
                                    format.FormatFlags = StringFormatFlags.NoWrap;
                                    format.Alignment = StringAlignment.Center;

                                    if (options.DropShadowLevel > 0)
                                    {
                                        switch (options.DropShadowLevel)
                                        {
                                            case 1:
                                                rectangle.Offset(-1, -1);
                                                graphics.DrawString(text, font, new SolidBrush(options.ShadowColor), rectangle,
                                                             format);
                                                rectangle.Offset(+1, +1);
                                                break;

                                            case 2:
                                                rectangle.Offset(+1, -1);
                                                graphics.DrawString(text, font, new SolidBrush(options.ShadowColor), rectangle,
                                                             format);
                                                rectangle.Offset(-1, +1);
                                                break;

                                            case 3:
                                                rectangle.Offset(-1, +1);
                                                graphics.DrawString(text, font, new SolidBrush(options.ShadowColor), rectangle,
                                                             format);
                                                rectangle.Offset(+1, -1);
                                                break;

                                            case 4:
                                                rectangle.Offset(+1, +1);
                                                graphics.DrawString(text, font, new SolidBrush(options.ShadowColor), rectangle,
                                                             format);
                                                rectangle.Offset(-1, -1);
                                                break;
                                        }
                                    }

                                    graphics.DrawString(text, font, fgBrush, rectangle, format);

                                    using (var memory = new MemoryStream())
                                    {
                                        pic.Save(memory, ImageFormat.Png);
                                        return memory.ToArray();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}