using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class SimpleXlsxReaderTests
{
    [TestMethod]
    public void VerifySimpleXlsxReaderWorks()
    {
        var rows = ReadHavaleh(filePath: "sample.xlsx");

        Assert.AreEqual(expected: 15, rows.Count);
        Assert.AreEqual(expected: "016/044/0028/1573/1/001", rows[index: 0].ShenaseShomare);
    }

    public static List<HavalehRowDto> ReadHavaleh(string filePath)
    {
        using var reader = new SimpleXlsxReader(filePath);

        var result = new List<HavalehRowDto>();

        foreach (var row in reader.ReadRows().Skip(count: 1))
        {
            result.Add(new HavalehRowDto
            {
                ShomareHavaleh = row.GetValue(column: 1),
                NameMoshtari = row.GetValue(column: 2),
                Jahat = row.GetValue(column: 3),
                Tozihat = row.GetValue(column: 4),
                ShenaseShomare = row.GetValue(column: 5),
                SharhKala = row.GetValue(column: 6),
                TedadKarton = row.GetValue(column: 7).ToInt(),
                TedadAdad = row.GetValue(column: 8).ToInt(),
                TarikhKhorooj = row.GetValue(column: 9)
            });
        }

        return result;
    }
}

public class HavalehRowDto
{
    public string ShomareHavaleh { get; set; }

    public string NameMoshtari { get; set; }

    public string Jahat { get; set; }

    public string Tozihat { get; set; }

    public string ShenaseShomare { get; set; }

    public string SharhKala { get; set; }

    public int TedadKarton { get; set; }

    public int TedadAdad { get; set; }

    public string TarikhKhorooj { get; set; }
}
