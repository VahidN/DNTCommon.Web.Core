using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

/// <summary>
///     این کلاس شامل آزمون‌های واحد برای متدهای کمکی کلاس FileSizeUnitsExtensions است.
/// </summary>
[TestClass]
public class FileSizeUnitsExtensionsTests
{
    // این تست بررسی می‌کند که آیا تبدیل از کیلوبایت به بایت به درستی انجام می‌شود.
    [TestMethod]
    public void ToBytesFromKilobytesShouldReturnCorrectValue()
    {
        // Arrange
        long size = 2;
        long expected = 2048;

        // Act
        var actual = size.ToBytes(FileSizeUnit.Kilobyte);

        // Assert
        Assert.AreEqual(expected, actual, message: "تبدیل کیلوبایت به بایت باید به درستی انجام شود.");
    }

    // این تست بررسی می‌کند که آیا تبدیل از مگابایت به بایت به درستی انجام می‌شود.
    [TestMethod]
    public void ToBytesFromMegabytesShouldReturnCorrectValue()
    {
        // Arrange
        var size = 1;
        var expected = 1024 * 1024;

        // Act
        var actual = size.ToBytes(FileSizeUnit.Megabyte);

        // Assert
        Assert.AreEqual(expected, actual, message: "تبدیل مگابایت به بایت باید به درستی انجام شود.");
    }

    // این تست بررسی می‌کند که آیا تبدیل از بایت به بایت، مقدار اصلی را برمی‌گرداند.
    [TestMethod]
    public void ToBytesFromBytesShouldReturnSameValue()
    {
        // Arrange
        var size = 150.5;
        var expected = 150.5;

        // Act
        var actual = size.ToBytes(FileSizeUnit.Byte);

        // Assert
        Assert.AreEqual(expected, actual, message: "تبدیل بایت به بایت باید همان مقدار اولیه را برگرداند.");
    }

    // این تست بررسی می‌کند که آیا تبدیل از یک واحد نامعتبر به بایت، مقدار صفر را برمی‌گرداند.
    [TestMethod]
    public void ToBytesFromInvalidUnitShouldReturnZero()
    {
        // Arrange
        long size = 100;
        var invalidUnit = (FileSizeUnit)99; // یک مقدار نامعتبر برای enum

        // Act
        var actual = size.ToBytes(invalidUnit);

        // Assert
        Assert.AreEqual(expected: 0, actual, message: "تبدیل از واحد نامعتبر باید صفر برگرداند.");
    }

    // این تست بررسی می‌کند که آیا تبدیل بایت به کیلوبایت به درستی انجام می‌شود.
    [TestMethod]
    public void ToKilobytesFromBytesShouldReturnCorrectValue()
    {
        // Arrange
        long size = 2048;
        long expected = 2;

        // Act
        var actual = size.ToKilobytes(FileSizeUnit.Byte);

        // Assert
        Assert.AreEqual(expected, actual, message: "تبدیل بایت به کیلوبایت باید به درستی انجام شود.");
    }

    // این تست بررسی می‌کند که آیا تبدیل مگابایت به کیلوبایت به درستی انجام می‌شود.
    [TestMethod]
    public void ToKilobytesFromMegabytesShouldReturnCorrectValue()
    {
        // Arrange
        var size = 1.5;
        var expected = 1.5 * 1024;

        // Act
        var actual = size.ToKilobytes(FileSizeUnit.Megabyte);

        // Assert
        Assert.AreEqual(expected, actual, message: "تبدیل مگابایت به کیلوبایت باید به درستی انجام شود.");
    }

    // این تست بررسی می‌کند که آیا تبدیل بایت به مگابایت به درستی انجام می‌شود.
    [TestMethod]
    public void ToMegabytesFromBytesShouldReturnCorrectValue()
    {
        // Arrange
        double size = 1572864; // 1.5 MB
        var expected = 1.5;

        // Act
        var actual = size.ToMegabytes(FileSizeUnit.Byte);

        // Assert
        Assert.AreEqual(expected, actual, message: "تبدیل بایت به مگابایت باید به درستی انجام شود.");
    }

    // این تست بررسی می‌کند که آیا تبدیل گیگابایت به مگابایت به درستی انجام می‌شود.
    [TestMethod]
    public void ToMegabytesFromGigabytesShouldReturnCorrectValue()
    {
        // Arrange
        var size = 2;
        var expected = 2 * 1024;

        // Act
        var actual = size.ToMegabytes(FileSizeUnit.Gigabyte);

        // Assert
        Assert.AreEqual(expected, actual, message: "تبدیل گیگابایت به مگابایت باید به درستی انجام شود.");
    }

    // این تست بررسی می‌کند که آیا تبدیل مگابایت به گیگابایت به درستی انجام می‌شود.
    [TestMethod]
    public void ToGigabytesFromMegabytesShouldReturnCorrectValue()
    {
        // Arrange
        double size = 2048;
        double expected = 2;

        // Act
        var actual = size.ToGigabytes(FileSizeUnit.Megabyte);

        // Assert
        Assert.AreEqual(expected, actual, message: "تبدیل مگابایت به گیگابایت باید به درستی انجام شود.");
    }

    // این تست بررسی می‌کند که آیا تبدیل ترابایت به گیگابایت به درستی انجام می‌شود.
    [TestMethod]
    public void ToGigabytesFromTerabytesShouldReturnCorrectValue()
    {
        // Arrange
        long size = 3;
        long expected = 3 * 1024;

        // Act
        var actual = size.ToGigabytes(FileSizeUnit.Terabyte);

        // Assert
        Assert.AreEqual(expected, actual, message: "تبدیل ترابایت به گیگابایت باید به درستی انجام شود.");
    }

    // این تست بررسی می‌کند که آیا تبدیل گیگابایت به ترابایت به درستی انجام می‌شود.
    [TestMethod]
    public void ToTerabytesFromGigabytesShouldReturnCorrectValue()
    {
        // Arrange
        double size = 5120; // 5 TB
        double expected = 5;

        // Act
        var actual = size.ToTerabytes(FileSizeUnit.Gigabyte);

        // Assert
        Assert.AreEqual(expected, actual, message: "تبدیل گیگابایت به ترابایت باید به درستی انجام شود.");
    }

    // این تست بررسی می‌کند که آیا متد BytesTo تبدیل به واحد مگابایت را به درستی انجام می‌دهد.
    [TestMethod]
    public void BytesToMegabytesShouldReturnCorrectValue()
    {
        // Arrange
        long size = 2097152; // 2 MB
        long expected = 2;

        // Act
        var actual = size.BytesTo(FileSizeUnit.Megabyte);

        // Assert
        Assert.AreEqual(expected, actual, message: "متد BytesTo باید تبدیل به مگابایت را به درستی انجام دهد.");
    }

    // این تست بررسی می‌کند که آیا متد KilobytesTo تبدیل به واحد گیگابایت را به درستی انجام می‌دهد.
    [TestMethod]
    public void KilobytesToGigabytesShouldReturnCorrectValue()
    {
        // Arrange
        long size = 1048576; // 1 GB
        long expected = 1;

        // Act
        var actual = size.KilobytesTo(FileSizeUnit.Gigabyte);

        // Assert
        Assert.AreEqual(expected, actual, message: "متد KilobytesTo باید تبدیل به گیگابایت را به درستی انجام دهد.");
    }

    // این تست بررسی می‌کند که آیا متد MegabytesTo تبدیل به واحد بایت را به درستی انجام می‌دهد.
    [TestMethod]
    public void MegabytesToBytesShouldReturnCorrectValue()
    {
        // Arrange
        var size = 3;
        var expected = 3 * 1024 * 1024;

        // Act
        var actual = size.MegabytesTo(FileSizeUnit.Byte);

        // Assert
        Assert.AreEqual(expected, actual, message: "متد MegabytesTo باید تبدیل به بایت را به درستی انجام دهد.");
    }

    // این تست بررسی می‌کند که آیا متد GigabytesTo تبدیل به واحد کیلوبایت را به درستی انجام می‌دهد.
    [TestMethod]
    public void GigabytesToKilobytesShouldReturnCorrectValue()
    {
        // Arrange
        long size = 1;
        long expected = 1024 * 1024;

        // Act
        var actual = size.GigabytesTo(FileSizeUnit.Kilobyte);

        // Assert
        Assert.AreEqual(expected, actual, message: "متد GigabytesTo باید تبدیل به کیلوبایت را به درستی انجام دهد.");
    }

    // این تست بررسی می‌کند که آیا متد TerabytesTo تبدیل به واحد مگابایت را به درستی انجام می‌دهد.
    [TestMethod]
    public void TerabytesToMegabytesShouldReturnCorrectValue()
    {
        // Arrange
        long size = 2;
        long expected = 2 * 1024 * 1024;

        // Act
        var actual = size.TerabytesTo(FileSizeUnit.Megabyte);

        // Assert
        Assert.AreEqual(expected, actual, message: "متد TerabytesTo باید تبدیل به مگابایت را به درستی انجام دهد.");
    }

    // این تست عملکرد متد MultiplyBy1024 را با یک بار ضرب کردن بررسی می‌کند.
    [TestMethod]
    public void MultiplyBy1024WithSingleTimeShouldReturnCorrectValue()
    {
        // Arrange
        var number = 5;
        var expected = 5120;

        // Act
        var actual = number.MultiplyBy1024();

        // Assert
        Assert.AreEqual(expected, actual, message: "ضرب در 1024 باید مقدار صحیح را برگرداند.");
    }

    // این تست عملکرد متد MultiplyBy1024 را با چند بار ضرب کردن بررسی می‌کند.
    [TestMethod]
    public void MultiplyBy1024WithMultipleTimesShouldReturnCorrectValue()
    {
        // Arrange
        long number = 2;
        byte times = 3; // 2 * 1024 * 1024 * 1024
        long expected = 2147483648;

        // Act
        var actual = number.MultiplyBy1024(times);

        // Assert
        Assert.AreEqual(expected, actual, message: "ضرب چندباره در 1024 باید مقدار صحیح را برگرداند.");
    }

    // این تست عملکرد متد DivideBy1024 را با یک بار تقسیم کردن بررسی می‌کند.
    [TestMethod]
    public void DivideBy1024WithSingleTimeShouldReturnCorrectValue()
    {
        // Arrange
        double number = 2048;
        double expected = 2;

        // Act
        var actual = number.DivideBy1024();

        // Assert
        Assert.AreEqual(expected, actual, message: "تقسیم بر 1024 باید مقدار صحیح را برگرداند.");
    }

    // این تست عملکرد متد DivideBy1024 را با چند بار تقسیم کردن بررسی می‌کند.
    [TestMethod]
    public void DivideBy1024WithMultipleTimesShouldReturnCorrectValue()
    {
        // Arrange
        long number = 2147483648;
        byte times = 2; // number / 1024 / 1024
        long expected = 2048;

        // Act
        var actual = number.DivideBy1024(times);

        // Assert
        Assert.AreEqual(expected, actual, message: "تقسیم چندباره بر 1024 باید مقدار صحیح را برگرداند.");
    }

    // این تست عملکرد متد Power را برای توان صفر بررسی می‌کند.
    [TestMethod]
    public void PowerWithZeroExponentShouldReturnOne()
    {
        // Arrange
        var baseValue = 10;
        byte exponent = 0;
        var expected = 1;

        // Act
        var actual = baseValue.Power(exponent);

        // Assert
        Assert.AreEqual(expected, actual, message: "هر عدد به توان صفر باید یک شود.");
    }

    // این تست عملکرد متد Power را برای توان مثبت بررسی می‌کند.
    [TestMethod]
    public void PowerWithPositiveExponentShouldReturnCorrectValue()
    {
        // Arrange
        var baseValue = 2;
        byte exponent = 10;
        var expected = 1024;

        // Act
        var actual = baseValue.Power(exponent);

        // Assert
        Assert.AreEqual(expected, actual, message: "محاسبه توان باید به درستی انجام شود.");
    }
}
