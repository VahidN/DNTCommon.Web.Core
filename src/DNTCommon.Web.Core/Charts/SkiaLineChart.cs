using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace DNTCommon.Web.Core;

/// <summary>
///     A custom line-chart component
/// </summary>
public class SkiaLineChart : SkiaChart
{
    /// <summary>
    ///     Its default value is 20f.
    /// </summary>
    public float AxisMarginLeft { set; get; } = 20f;

    /// <summary>
    ///     Its default value is 20f
    /// </summary>
    public float AxisMarginRight { set; get; } = 20f;

    /// <summary>
    ///     Its default value is `SKColors.LightGray`.
    /// </summary>
    public SKColor AxisColor { get; set; } = SKColors.LightGray;

    /// <summary>
    ///     Its default value is `SKColors.DimGray`.
    /// </summary>
    public SKColor LinesColor { get; set; } = SKColors.DimGray;

    /// <summary>
    ///     Its default value is `20f`.
    /// </summary>
    public float LabelsMarginBottom { set; get; } = 20f;

    /// <summary>
    ///     Its default value is 8f
    /// </summary>
    public float PointsStrokeWidth { get; set; } = 8f;

    /// <summary>
    ///     Its default value 4f.
    /// </summary>
    public float PointsRadius { set; get; } = 2f;

    /// <summary>
    ///     Its default value is `20f`.
    /// </summary>
    public float AxisMarginBottom { set; get; } = 20f;

    /// <summary>
    ///     Its default value is 2f
    /// </summary>
    public float LinesStrokeWidth { set; get; } = 2f;

    /// <summary>
    ///     Draws a line-chart and returns it as a .png byte array by default
    /// </summary>
    public override byte[] Draw()
    {
        using var bitmap = new SKBitmap(Image.Width, Image.Height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(Image.BackgroundColor ?? SKColors.White);

        var startY = Image.Height - AxisMarginBottom;

        DrawLines(canvas, startY);

        DrawXYAxis(canvas, startY);

        if (Title is not null)
        {
            canvas.DrawTopAlignedText(Image.Width, Title.Text, Title.Font, Title.TextAlign, Title.Margin);
        }

        if (Image.ShowFrame)
        {
            canvas.DrawFrame(Image.FrameColor, Image.Width - 1, Image.Height - 1);
        }

        return bitmap.ToImageBytes(Image.Format, Image.Quality);
    }

    private void DrawLines(SKCanvas canvas, float startY)
    {
        var labelFontType = LabelsFont.Name.GetFont(LabelsFont.Style, LabelsFont.FilePath);
        using var labelShaper = new SKShaper(labelFontType);

        using var labelFont = new SKFont();
        labelFont.Size = LabelsFont.Size;
        labelFont.Typeface = labelFontType;

        using var labelPaint = new SKPaint();
        labelPaint.IsAntialias = true;

        var maxHeight = Image.Height - (5 * LabelsMarginBottom);
        var maxValue = Items.Max(x => x.Value);
        var itemsCount = Items.Count;
        var stepX = (Image.Width - (2 * AxisMarginLeft)) / itemsCount;

        using var linePaint = new SKPaint();
        linePaint.Color = LinesColor;
        linePaint.StrokeWidth = LinesStrokeWidth;
        linePaint.IsAntialias = true;
        linePaint.Style = SKPaintStyle.Stroke;

        using var pointPaint = new SKPaint();
        pointPaint.StrokeWidth = PointsStrokeWidth;
        pointPaint.IsAntialias = true;
        pointPaint.Style = SKPaintStyle.StrokeAndFill;

        for (var i = 0; i < itemsCount - 1; i++)
        {
            var x1 = AxisMarginLeft + (i * stepX);
            var y1 = startY - (Items[i].Value / maxValue * maxHeight);

            var x2 = AxisMarginLeft + ((i + 1) * stepX);
            var y2 = startY - (Items[i + 1].Value / maxValue * maxHeight);

            canvas.DrawLine(x1, y1, x2, y2, linePaint);

            pointPaint.Color = UseItemColorsForLabels ? Items[i].Color ?? LabelsFont.Color : LabelsFont.Color;
            canvas.DrawCircle(x1, y1, PointsRadius, pointPaint);

            labelPaint.Color = UseItemColorsForLabels ? Items[i].Color ?? LabelsFont.Color : LabelsFont.Color;

            DrawLabel(canvas, labelShaper, labelFont, labelPaint, Items[i].Label, Items[i].Value, x1, y1);

            if (i == itemsCount - 2)
            {
                pointPaint.Color = UseItemColorsForLabels ? Items[i + 1].Color ?? LabelsFont.Color : LabelsFont.Color;
                canvas.DrawCircle(x2, y2, PointsRadius, pointPaint);

                labelPaint.Color = UseItemColorsForLabels ? Items[i + 1].Color ?? LabelsFont.Color : LabelsFont.Color;

                DrawLabel(canvas, labelShaper, labelFont, labelPaint, Items[i + 1].Label, Items[i + 1].Value, x2, y2);
            }
        }
    }

    private void DrawLabel(SKCanvas canvas,
        SKShaper labelShaper,
        SKFont labelFont,
        SKPaint labelPaint,
        string? label,
        float value,
        float x,
        float y)
    {
        canvas.DrawShapedText(labelShaper, label, x + (4 * PointsRadius), y, SKTextAlign.Left, labelFont, labelPaint);

        var textWidth = label.GetTextWidth(labelFont);

        canvas.DrawShapedText(labelShaper, $" ({GetFormattedNumber(value)})", x + (4 * PointsRadius) + textWidth, y,
            SKTextAlign.Left, labelFont, labelPaint);
    }

    private void DrawXYAxis(SKCanvas canvas, float startY)
    {
        using var axisPaint = new SKPaint();
        axisPaint.Color = AxisColor;
        axisPaint.StrokeWidth = 1;
        axisPaint.IsAntialias = true;

        canvas.DrawLine(AxisMarginLeft, startY, Image.Width - AxisMarginRight, startY, axisPaint);
        canvas.DrawLine(AxisMarginLeft, startY, AxisMarginLeft, AxisMarginRight, axisPaint);
    }
}
