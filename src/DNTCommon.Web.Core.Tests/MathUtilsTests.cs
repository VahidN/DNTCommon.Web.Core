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
}
