using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class StringUtilsTests : TestsBase
{
    [TestMethod]
    public void TestTrimStart()
        => Assert.AreEqual(expected: "xyz test",
            "tests 1 xyz test".TrimStart(value: "tests 1", StringComparison.Ordinal));

    [TestMethod]
    public void TestTrimEnd()
        => Assert.AreEqual(expected: "tests 1",
            "tests 1 xyz test".TrimEnd(value: "xyz test", StringComparison.Ordinal));
}