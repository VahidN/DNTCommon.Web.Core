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

    [TestMethod]
    [DataRow(data: "ｅｘｐｒｅｓｓｉｏｎ")]
    [DataRow(data: "ｅ")]
    [DataRow(data: "ＥＸＰＲＥＳＳＩＯＮ")]
    public void TestIsFullWidth(string input) => Assert.IsTrue(input.HasFullWidthChar());

    [TestMethod]
    [DataRow(data: "expression")]
    [DataRow(data: "e")]
    public void TestIsNotFullWidth(string input) => Assert.IsFalse(input.HasFullWidthChar());

    [DataTestMethod]
    [DataRow(data: 'ᴀ')]
    [DataRow(data: 'ʙ')]
    [DataRow(data: 'ᴄ')]
    [DataRow(data: 'ᴅ')]
    [DataRow(data: 'ᴇ')]
    [DataRow(data: 'ғ')]
    [DataRow(data: 'ɢ')]
    [DataRow(data: 'ʜ')]
    [DataRow(data: 'ɪ')]
    [DataRow(data: 'ᴊ')]
    [DataRow(data: 'ᴋ')]
    [DataRow(data: 'ʟ')]
    [DataRow(data: 'ᴍ')]
    [DataRow(data: 'ɴ')]
    [DataRow(data: 'ᴏ')]
    [DataRow(data: 'ᴘ')]
    [DataRow(data: 'ʀ')]
    [DataRow(data: 'ᴛ')]
    [DataRow(data: 'ᴜ')]
    [DataRow(data: 'ᴡ')]
    [DataRow(data: 'ʏ')]
    [DataRow(data: 'ᴢ')]
    public void TestSmallCapitalLetters(char c)
        => Assert.IsTrue(c.IsSmallCapitalChar(), $"Character {c} should be detected as a small capital.");
}