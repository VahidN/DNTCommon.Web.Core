using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace DNTCommon.Web.Core;

/// <summary>
///     A custom pie-chart component
/// </summary>
public class SkiaPieChart : SkiaChart
{
    /// <summary>
    ///     Its default value is `200`
    /// </summary>
    public float PieRadius { set; get; } = 200f;

    /// <summary>
    ///     Its default value is 220.
    /// </summary>
    public float LabelPieRadius { set; get; } = 220;

    /// <summary>
    ///     Draws a pie-chart and returns it as a .png byte array by default
    /// </summary>
    public override byte[] Draw()
    {
        using var bitmap = new SKBitmap(Image.Width, Image.Height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(Image.BackgroundColor ?? SKColors.White);

        var centerX = (float)Image.Width / 2;
        var centerY = (float)Image.Height / 2;
        float startAngle = 0;
        var totalSum = Items.Sum(chartItem => chartItem.Value);

        var labelFontType = LabelsFont.Name.GetFont(LabelsFont.Style, LabelsFont.FilePath);
        using var labelShaper = new SKShaper(labelFontType);

        using var labelFont = new SKFont();
        labelFont.Size = LabelsFont.Size;
        labelFont.Typeface = labelFontType;

        foreach (var chartItem in Items)
        {
            var sweepAngle = DrawPie(canvas, chartItem, totalSum, centerX, centerY, startAngle);

            DrawLabel(canvas, chartItem, centerX, centerY, startAngle, sweepAngle, labelShaper, labelFont);

            startAngle += sweepAngle;
        }

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

    private float DrawPie(SKCanvas canvas,
        ChartItem chartItem,
        float totalSum,
        float centerX,
        float centerY,
        float startAngle)
    {
        using var piePaint = new SKPaint();

        piePaint.IsAntialias = true;
        piePaint.Style = SKPaintStyle.Fill;

        var sweepAngle = chartItem.Value / totalSum * 360;
        piePaint.Color = chartItem.Color ?? RandomSKColorProvider.RandomSkColor;

        using var piePath = new SKPath();
        piePath.MoveTo(centerX, centerY);

        piePath.ArcTo(new SKRect(centerX - PieRadius, centerY - PieRadius, centerX + PieRadius, centerY + PieRadius),
            startAngle, sweepAngle, forceMoveTo: false);

        piePath.Close();
        canvas.DrawPath(piePath, piePaint);

        return sweepAngle;
    }

    private void DrawLabel(SKCanvas canvas,
        ChartItem chartItem,
        float centerX,
        float centerY,
        float startAngle,
        float sweepAngle,
        SKShaper labelShaper,
        SKFont labelFont)
    {
        var labelAngle = startAngle + (sweepAngle / 2);
        var labelX = centerX + (LabelPieRadius * (float)Math.Cos(labelAngle * Math.PI / 180));
        var labelY = centerY + (LabelPieRadius * (float)Math.Sin(labelAngle * Math.PI / 180));

        using var labelPaint = new SKPaint();

        labelPaint.Color = UseItemColorsForLabels ? chartItem.Color ?? LabelsFont.Color : LabelsFont.Color;

        labelPaint.IsAntialias = true;
        labelPaint.Style = SKPaintStyle.Fill;

        var textWidth = chartItem.Label.GetTextWidth(labelFont);

        if (labelX < centerX)
        {
            labelX -= textWidth + (2 * (LabelPieRadius - PieRadius));
        }

        canvas.DrawShapedText(labelShaper, chartItem.Label, labelX, labelY, labelFont, labelPaint);

        canvas.DrawShapedText(labelShaper, $" ({GetFormattedNumber(chartItem.Value)})", labelX + textWidth, labelY,
            labelFont, labelPaint);
    }
}
