using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class ChartsController(IWebHostEnvironment env) : Controller
{
    public IActionResult Index() => View();

    public IActionResult PieChart()
    {
        var skiaPieChart = new SkiaPieChart
        {
            Items = GetItems(),
            Title = new ChartTitle
            {
                Text = "نمودار پاي است",
                Font = new ChartFont
                {
                    FilePath = env.WebRootPath.SafePathCombine("fonts", "tahoma.ttf"),
                    Style = SKFontStyle.Bold,
                    Size = 20
                }
            },
            LabelsFont = new ChartFont
            {
                Name = "Tahoma"
            },
            UseItemColorsForLabels = true,
            Image = new ChartImage
            {
                Width = 700,
                Height = 700,
                ShowFrame = true
            }
        };

        return File(skiaPieChart.DrawAsStream(), skiaPieChart.Image.Format.ToMimeType());
    }

    public IActionResult BarChart()
    {
        var skiaVerticalBarChart = new SkiaVerticalBarChart
        {
            Items = GetItems(),
            Title = new ChartTitle
            {
                Text = "نمودار ميله‌اي است",
                Font = new ChartFont
                {
                    FilePath = env.WebRootPath.SafePathCombine("fonts", "tahoma.ttf"),
                    Style = SKFontStyle.Bold,
                    Size = 20
                }
            },
            LabelsFont = new ChartFont
            {
                Name = "Tahoma"
            },
            UseItemColorsForLabels = true,
            Image = new ChartImage
            {
                Width = 700,
                Height = 700,
                ShowFrame = true
            }
        };

        return File(skiaVerticalBarChart.DrawAsStream(), skiaVerticalBarChart.Image.Format.ToMimeType());
    }

    public IActionResult LineChart()
    {
        var skiaLineChart = new SkiaLineChart
        {
            Items = GetItems(),
            Title = new ChartTitle
            {
                Text = "نمودار خطي است",
                Font = new ChartFont
                {
                    FilePath = env.WebRootPath.SafePathCombine("fonts", "tahoma.ttf"),
                    Style = SKFontStyle.Bold,
                    Size = 20
                }
            },
            LabelsFont = new ChartFont
            {
                Name = "Tahoma"
            },
            UseItemColorsForLabels = true,
            Image = new ChartImage
            {
                Width = 700,
                Height = 700,
                ShowFrame = true
            }
        };

        return File(skiaLineChart.DrawAsStream(), skiaLineChart.Image.Format.ToMimeType());
    }

    private static List<ChartItem> GetItems()
        =>
        [
            new(label: "App Store", value: 30, RandomSKColorProvider.RandomSkColor),
            new(label: "وب سايت", value: 170, RandomSKColorProvider.RandomSkColor),
            new(label: "Partners", value: 42, RandomSKColorProvider.RandomSkColor),
            new(label: "مستقيم", value: 50, RandomSKColorProvider.RandomSkColor),
            new(label: "Channels", value: 3, RandomSKColorProvider.RandomSkColor),
            new(label: "Retail", value: 55, RandomSKColorProvider.RandomSkColor),
            new(label: "Distributors", value: 35, RandomSKColorProvider.RandomSkColor),
            new(label: "Affiliates", value: 22, RandomSKColorProvider.RandomSkColor),
            new(label: "Phone", value: 10, RandomSKColorProvider.RandomSkColor),
            new(label: "TV", value: 5, RandomSKColorProvider.RandomSkColor)
        ];
}
