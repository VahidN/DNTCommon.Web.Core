using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class FormatSizeTests
{
    [TestMethod]
    public void ToFormattedFileSizeShouldReturnCorrectValue()
    {
        // Arrange
        long size = 2048;

        // Act
        var actual = size.ToFormattedFileSize();

        // Assert
        Assert.AreEqual(expected: "2 KB", actual);
    }

    [TestMethod]
    public void ToFormattedFileSizeShouldReturnCorrectValueFor45MB()
    {
        // Arrange
        long size = 47_185_920;

        // Act
        var actual = size.ToFormattedFileSize();

        // Assert
        Assert.AreEqual(expected: "45 MB", actual);
    }
}
