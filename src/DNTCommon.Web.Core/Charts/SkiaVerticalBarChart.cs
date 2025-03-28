using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace DNTCommon.Web.Core;

/// <summary>
///     A custom bar-chart component
/// </summary>
public class SkiaVerticalBarChart : SkiaChart
{
    /// <summary>
    ///     Its default value is `SKColors.LightGray`.
    /// </summary>
    public SKColor AxisColor { get; set; } = SKColors.LightGray;

    /// <summary>
    ///     Its default value is `50f`.
    /// </summary>
    public float AxisMarginLeft { set; get; } = 50f;

    /// <summary>
    ///     Its default value is `60f`.
    /// </summary>
    public float AxisMarginBottom { set; get; } = 60f;

    /// <summary>
    ///     Its default value is `30f`.
    /// </summary>
    public float LabelsMarginBottom { set; get; } = 30f;

    /// <summary>
    ///     Its default value is 10
    /// </summary>
    public float BarsGapWidth { set; get; } = 10;

    /// <summary>
    ///     Its default value is 45f
    /// </summary>
    public float LabelsRotateDegrees { set; get; } = 45f;

    /// <summary>
    ///     Its default value is 20f
    /// </summary>
    public float AxisMarginRight { set; get; } = 20f;

    /// <summary>
    ///     Its default value is 10f
    /// </summary>
    public float ValueLabelsMarginBottom { set; get; } = 10f;

    /// <summary>
    ///     Draws a bar-chart and returns it as a .png byte array by default
    /// </summary>
    public override byte[] Draw()
    {
        using var bitmap = new SKBitmap(Image.Width, Image.Height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(Image.BackgroundColor ?? SKColors.White);

        var barWidth = (Image.Width - (AxisMarginLeft * 2)) / Items.Count;
        var maxValue = Items.Max(x => x.Value);
        var scale = (Image.Height - (AxisMarginLeft * 2)) / maxValue;
        var startY = Image.Height - AxisMarginBottom;

        var labelFontType = LabelsFont.Name.GetFont(LabelsFont.Style, LabelsFont.FilePath);
        using var labelShaper = new SKShaper(labelFontType);

        using var labelFont = new SKFont();
        labelFont.Size = LabelsFont.Size;
        labelFont.Typeface = labelFontType;

        for (var i = 0; i < Items.Count; i++)
        {
            var x = AxisMarginLeft + (i * barWidth);

            DrawBar(i, canvas, labelFont, labelShaper, x, barWidth, scale, startY);

            DrawLabel(i, canvas, labelFont, labelShaper, x, barWidth);
        }

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

    private void DrawBar(int i,
        SKCanvas canvas,
        SKFont labelFont,
        SKShaper labelShaper,
        float x,
        float barWidth,
        float scale,
        float startY)
    {
        using var barPaint = new SKPaint();
        barPaint.Color = Items[i].Color ?? SKColors.Blue;
        barPaint.IsAntialias = true;

        var barHeight = Items[i].Value * scale;
        var y = startY - barHeight;

        canvas.DrawRect(x, y, barWidth - BarsGapWidth, barHeight, barPaint);

        canvas.DrawShapedText(labelShaper, GetFormattedNumber(Items[i].Value), x + (barWidth / 2),
            y - ValueLabelsMarginBottom, LabelsTextAlign, labelFont, barPaint);
    }

    private void DrawLabel(int i, SKCanvas canvas, SKFont labelFont, SKShaper labelShaper, float x, float barWidth)
    {
        using var labelPaint = new SKPaint();
        labelPaint.Color = UseItemColorsForLabels ? Items[i].Color ?? LabelsFont.Color : LabelsFont.Color;
        labelPaint.IsAntialias = true;
        labelPaint.Style = SKPaintStyle.Fill;

        var tx = x + (barWidth / 2);
        var ty = Image.Height - LabelsMarginBottom;

        canvas.Save();
        canvas.RotateDegrees(LabelsRotateDegrees, tx, ty);
        canvas.DrawShapedText(labelShaper, Items[i].Label, tx, ty, LabelsTextAlign, labelFont, labelPaint);
        canvas.Restore();
    }

    private void DrawXYAxis(SKCanvas canvas, float startY)
    {
        using var axisPaint = new SKPaint();
        axisPaint.Color = AxisColor;
        axisPaint.StrokeWidth = 1;
        axisPaint.IsAntialias = true;

        canvas.DrawLine(AxisMarginLeft - BarsGapWidth, startY, Image.Width - AxisMarginRight, startY, axisPaint);

        canvas.DrawLine(AxisMarginLeft - BarsGapWidth, startY, AxisMarginLeft - BarsGapWidth, AxisMarginRight,
            axisPaint);
    }
}
