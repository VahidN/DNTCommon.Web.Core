using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class MathUtilsTests : TestsBase
{
    [TestMethod]
    public void TestCountDigits()
    {
        Assert.AreEqual(expected: 3, 123.CountDigits());
        Assert.AreEqual(expected: 4, 1234.CountDigits());
    }

    [TestMethod]
    public void TestToStringPadLeftWithZero()
    {
        Assert.AreEqual(expected: "00123", 123.ToStringPadLeft(totalWidth: 5));
        Assert.AreEqual(expected: "01234", 1234.ToStringPadLeft(totalWidth: 5));
        Assert.AreEqual(expected: "12345", 12345.ToStringPadLeft(totalWidth: 5));
    }
}
