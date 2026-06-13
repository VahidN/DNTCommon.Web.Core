using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class FormatSizeTests
{
    [TestMethod]
    [DataRow(2048, "2 KB")]
    [DataRow(47_185_920, "45 MB")]
    [DataRow(4_106_104_832, "3.8 GB")]
    public void ToFormattedFileSizeShouldReturnCorrectValue(long size, string expected)
        => Assert.AreEqual(expected, size.ToFormattedFileSize());
}
