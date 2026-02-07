using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class StringUtilsTests : TestsBase
{
    [TestMethod]
    public void TestTrimStart()
    {
        Assert.AreEqual(expected: "xyz test", "tests 1 xyz test".TrimStart(value: "tests 1", StringComparison.Ordinal));

        Assert.AreEqual(expected: "gemma-3n-e4b-it",
            "models/gemma-3n-e4b-it".TrimStart(value: "models/", StringComparison.Ordinal));

        Assert.AreEqual(expected: "gemma-3n-e4b-it",
            "gemma-3n-e4b-it".TrimStart(value: "models/", StringComparison.Ordinal));
    }

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

    [TestMethod]
    [DataRow("this is a test", 2, "is is a test")]
    [DataRow("test", 4, "")]
    [DataRow("test", 6, "")]
    [DataRow("test", 1, "est")]
    public void TestRemoveFirstNCharsWorks(string input, int len, string output)
        => Assert.AreEqual(output, input.RemoveFirstNChars(len));

    [TestMethod]
    [DataRow("this is a test", 2, "this is a te")]
    [DataRow("test", 4, "")]
    [DataRow("test", 6, "")]
    [DataRow("test", 1, "tes")]
    public void TestRemoveLastNCharsWorks(string input, int len, string output)
        => Assert.AreEqual(output, input.RemoveLastNChars(len));

    [TestMethod]
    [DataRow("this is a test", 2, "th")]
    [DataRow("test", 4, "test")]
    [DataRow("test", 6, "test")]
    [DataRow("test", 1, "t")]
    public void TestTakeFirstNCharsWorks(string input, int len, string output)
        => Assert.AreEqual(output, input.TakeFirstNChars(len));

    [TestMethod]
    [DataRow("this is a test", 2, "st")]
    [DataRow("test", 4, "test")]
    [DataRow("test", 6, "test")]
    [DataRow("test", 1, "t")]
    public void TestTakeLastNCharsWorks(string input, int len, string output)
        => Assert.AreEqual(output, input.TakeLastNChars(len));
}
